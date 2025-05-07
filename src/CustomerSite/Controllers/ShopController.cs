using CustomerSite.Models;
using CustomerSite.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using SharedViewModels.Review;

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
        if (!response.Succeeded)
        {
            TempData["ErrorMessage"] = response.Message;
            return RedirectToAction("Index", "Home");
        }
        
        return View(response.Data);
    }

    [Route("shop/c/{categoryId}")]
    public async Task<IActionResult> ProductList(Guid categoryId)
    {
        var categoryResponse = await _categoryService.GetCategoryByIdAsync(categoryId);
        var productsResponse = await _productService.GetProductsByCategory(categoryId);
        
        if (!categoryResponse.Succeeded || !productsResponse.Succeeded)
        {
            TempData["ErrorMessage"] = categoryResponse.Message;
            return RedirectToAction("Index");
        }
        
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

    [HttpPost]
    public async Task<IActionResult> SubmitReview(CreateReviewRequest request)
    {
        if (!User.Identity.IsAuthenticated)
        {
            return Unauthorized();
        }
        string accessToken = await HttpContext.GetTokenAsync(
            OpenIdConnectDefaults.AuthenticationScheme, "access_token");

        var response = await _reviewService.CreateReview(request, accessToken);
    
        if (response.Succeeded)
        {
            return RedirectToAction("ProductDetail", new { productId = request.ProductId });
        }
    
        TempData["ErrorMessage"] = response.Message;
        return RedirectToAction("ProductDetail", new { productId = request.ProductId });
    }

    

}