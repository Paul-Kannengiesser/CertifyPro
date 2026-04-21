using CertifyPro.Models;
using CertifyPro.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;

namespace CertifyPro.Controllers;

[Authorize]
public class SettingsController : Controller
{
    private readonly CompanySettingsService _companySettings;

    public SettingsController(CompanySettingsService companySettings)
    {
        _companySettings = companySettings;
    }

    private string UserId => User.GetObjectId()
        ?? throw new InvalidOperationException("User not authenticated.");

    // GET /Settings
    public async Task<IActionResult> Index(bool onboarding = false)
    {
        var settings = await _companySettings.GetAsync(UserId) ?? new CompanySettings();

        // Onboarding mode: no existing settings or explicitly flagged
        var isOnboarding = onboarding || string.IsNullOrWhiteSpace(settings.Firmenname);
        ViewData["Onboarding"] = isOnboarding;

        return View(settings);
    }

    // POST /Settings
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(CompanySettings settings, bool onboarding = false)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Onboarding"] = onboarding;
            return View(settings);
        }

        await _companySettings.SaveAsync(settings, UserId);

        if (onboarding)
            return RedirectToAction("Index", "Evaluation");

        TempData["Success"] = "Einstellungen gespeichert.";
        return RedirectToAction(nameof(Index));
    }
}
