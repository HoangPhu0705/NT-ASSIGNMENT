using Microsoft.AspNetCore.Mvc;

namespace CustomerSite.ViewComponents;

public class CategoryCardViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(string imageUrl, string categoryName, string href)
    {
        var model = new CategoryCardViewModel
        {
            imageUrl = imageUrl,
            categoryName = categoryName,
            href = href
        };
        
        return View(model);
    }
}

public class CategoryCardViewModel
{
    public string imageUrl { get; set; }
    public string categoryName { get; set; }
    
    public string href { get; set; }
}