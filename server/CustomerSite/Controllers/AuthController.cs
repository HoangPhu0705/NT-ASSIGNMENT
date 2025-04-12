using Microsoft.AspNetCore.Mvc;

namespace CustomerSite.Controllers;

public class AuthController : Controller
{
    public IActionResult Login()
    {
        return View();
    }
}