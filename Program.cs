using CertifyPro.Filters;
using CertifyPro.Repositories;
using CertifyPro.Services;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Microsoft Identity authentication
builder.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration, "AzureAd");

// Fix sign-out: skip account picker + redirect back to our app
builder.Services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    options.Events.OnRedirectToIdentityProviderForSignOut = context =>
    {
        // logout_hint skips the "which account?" picker
        var logoutHint = context.HttpContext.User.FindFirst("login_hint")?.Value
            ?? context.HttpContext.User.FindFirst("preferred_username")?.Value;
        if (!string.IsNullOrEmpty(logoutHint))
            context.ProtocolMessage.SetParameter("logout_hint", logoutHint);

        // Must point to a registered redirect URI — the middleware then
        // handles /signout-callback-oidc and redirects to SignedOutRedirectUri (/)
        var baseUrl = $"{context.Request.Scheme}://{context.Request.Host}";
        context.ProtocolMessage.PostLogoutRedirectUri = $"{baseUrl}/signout-callback-oidc";

        return Task.CompletedTask;
    };
});

// MVC with global [Authorize] policy — exceptions use [AllowAnonymous]
builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
    options.Filters.AddService<OnboardingFilter>();
}).AddMicrosoftIdentityUI();

// CertifyPro services
builder.Services.AddSingleton<CompanySettingsService>();
builder.Services.AddSingleton<EvaluationPromptGenerator>();
builder.Services.AddScoped<IEvaluationFormulationService, AiEvaluationFormulationService>();
builder.Services.AddSingleton<IEvaluationRepository, JsonEvaluationRepository>();

// Register filter for DI
builder.Services.AddScoped<OnboardingFilter>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Index");
    app.UseHsts();
}

if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
