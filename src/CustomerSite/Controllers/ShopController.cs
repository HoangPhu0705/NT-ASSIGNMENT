using CustomerSite.Services;
using Microsoft.AspNetCore.Mvc;

namespace CustomerSite.Controllers;

public class ShopController : Controller
{
    private readonly CategoryService _categoryService;
    private readonly ProductService _productService;

    public ShopController(CategoryService categoryService, ProductService productService)
    {
        _categoryService = categoryService;
        _productService = productService;
    }
    
    public async Task<IActionResult> Index()
    {
        var response = await _categoryService.GetCategoriesAsync();
        return View(response.Data);
    }

    [Route("shop/{categoryId}")]
    public async Task<IActionResult> ProductList(int categoryId)
    {
        return View();
    }
    

    
    
    
    
}