using CustomerSite.Models;
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

    [Route("shop/c/{categoryId}")]
    public async Task<IActionResult> ProductList(Guid categoryId)
    {
        var categoryResponse = await _categoryService.GetCategoryByIdAsync(categoryId);
        var productsResponse = await _productService.GetProductsByCategory(categoryId);
    
        var viewModel = new ShopViewModel()
        {
            Category = categoryResponse.Data,
            Products = productsResponse.Data
        };
    
        return View(viewModel);
    }

    [Route("shop/p/{productId}")]
    public async Task<IActionResult> ProductDetail(Guid productId)
    {
        var productResponse = await _productService.GetProductDetail(productId);
        return View(productResponse.Data);
    }
    
}