using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

namespace CustomerSite.Controllers;

public class CartController : Controller
{
    public IActionResult Index()
    {
        if (!User.Identity.IsAuthenticated)
        {
            return Challenge(new AuthenticationProperties
                {
                    RedirectUri = Url.Action(nameof(Index), "Cart"),
                },
                OpenIdConnectDefaults.AuthenticationScheme);
        }
        
        return View();
    } 
}