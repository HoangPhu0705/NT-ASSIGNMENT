using CustomerSite.Models;
using CustomerSite.Services;
using Microsoft.AspNetCore.Mvc;

namespace CustomerSite.Controllers;

public class ShopController : Controller
{
    private readonly CategoryService _categoryService;
    private readonly ProductService _productService;
    private readonly ReviewService _reviewService;

    public ShopController(CategoryService categoryService, ProductService productService, ReviewService reviewService)
    {
        _categoryService = categoryService;
        _productService = productService;
        _reviewService = reviewService;
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
        var reviewResponse = await _reviewService.GetReviewsByProductId(productId);
        var viewModel = new ProductDetailModel()
        {
            Product = productResponse.Data,
            Review = reviewResponse.Data
        };
        return View(viewModel);
    }
    
}