using CustomerSite.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using SharedViewModels.Cart;

namespace CustomerSite.Controllers;

public class CartController : Controller
{
    private readonly CartService _cartService;  
    
    public CartController(CartService cartService)
    {
        _cartService = cartService;
    }
    
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
        
        var accessToken = HttpContext.GetTokenAsync(OpenIdConnectDefaults.AuthenticationScheme, "access_token").Result;
        var response = _cartService.GetCartAsync(accessToken).Result;
        
        if (!response.Succeeded)
        {
            TempData["ErrorMessage"] = response.Message;
            return RedirectToAction("Index", "Cart");
        }

        return View(response.Data);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
    {
        if (!User.Identity.IsAuthenticated)
        {
            return Json(new { success = false, message = "You must be logged in to add items to cart." });
        }
    
        string accessToken = await HttpContext.GetTokenAsync(
            OpenIdConnectDefaults.AuthenticationScheme, "access_token");
        
        var response = await _cartService.AddToCartAsync(request, accessToken);
        
        Console.WriteLine("Yooo" + response.Data);
        
        return Json(new { 
            success = response.Succeeded, 
            message = response.Succeeded ? "Item added to cart successfully!" : response.Message 
        });
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateCartItem([FromBody] UpdateCartItemRequest request)
    {
        if (!User.Identity.IsAuthenticated)
        {
            return Json(new { success = false, message = "You must be logged in to update items in cart." });
        }
    
        string accessToken = await HttpContext.GetTokenAsync(
            OpenIdConnectDefaults.AuthenticationScheme, "access_token");
        
        var response = await _cartService.UpdateCartItemAsync(request, accessToken);
        
        return Json(new { 
            success = response.Succeeded, 
            message = response.Succeeded ? "Cart item updated successfully!" : response.Message 
        });
    }
    
    [HttpDelete]
    public async Task<IActionResult> RemoveCartItem([FromQuery] Guid itemId)
    {
        Console.WriteLine($"RemoveCartItem called with itemId: {itemId}");
        if (!User.Identity.IsAuthenticated)
        {
            return Json(new { success = false, message = "You must be logged in to remove items from cart." });
        }
        
        string accessToken = await HttpContext.GetTokenAsync(
            OpenIdConnectDefaults.AuthenticationScheme, "access_token") ?? throw new InvalidOperationException("Access token not found.");
        
        var response = await _cartService.RemoveCartItemAsync(itemId, accessToken);
        
        return Json(new { 
            success = response.Succeeded, 
            message = response.Succeeded ? "Cart item removed successfully!" : response.Message 
        });
        
    }
}