using CertifyPro.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Identity.Web;

namespace CertifyPro.Filters;

public class OnboardingFilter : IAsyncActionFilter
{
    private readonly CompanySettingsService _companySettings;

    public OnboardingFilter(CompanySettingsService companySettings)
    {
        _companySettings = companySettings;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var user = context.HttpContext.User;

        // Only applies to authenticated users
        if (user.Identity?.IsAuthenticated != true)
        {
            await next();
            return;
        }

        // Don't redirect when already on the Settings page (avoid loop)
        var controller = context.RouteData.Values["controller"]?.ToString();
        if (string.Equals(controller, "Settings", StringComparison.OrdinalIgnoreCase))
        {
            await next();
            return;
        }

        var userId = user.GetObjectId();
        if (userId == null)
        {
            await next();
            return;
        }

        var settings = await _companySettings.GetAsync(userId);
        if (settings == null || string.IsNullOrWhiteSpace(settings.Firmenname))
        {
            context.Result = new RedirectToActionResult("Index", "Settings", new { onboarding = true });
            return;
        }

        await next();
    }
}
