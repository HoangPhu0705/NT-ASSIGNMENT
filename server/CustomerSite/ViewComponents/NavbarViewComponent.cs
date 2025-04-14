using Microsoft.AspNetCore.Mvc;

namespace CustomerSite.ViewComponents;

public class NavbarViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View();
    }
}