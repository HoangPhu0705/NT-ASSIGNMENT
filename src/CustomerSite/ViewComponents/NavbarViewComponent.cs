using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CustomerSite.ViewComponents
{
    public class NavbarViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            // Use HttpContext.User, which is already a ClaimsPrincipal
            var claimsPrincipal = HttpContext.User;
            return View(claimsPrincipal);
        }
    }
}