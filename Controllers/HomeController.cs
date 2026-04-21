using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CertifyPro.Controllers;

public class HomeController : Controller
{
    // Public landing page — no [Authorize]
    [AllowAnonymous]
    public IActionResult Index()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Evaluation");

        return View();
    }
}
