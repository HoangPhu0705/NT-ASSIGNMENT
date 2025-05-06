using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces.Carts;
using SharedViewModels.Cart;
using SharedViewModels.Shared;
using System.Security.Claims;
using OpenIddict.Validation.AspNetCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/cart")]
    [Authorize(
        AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme,
        Roles = "User")]    
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<CartDto>>> GetCart()
        {
            try
            {
                var userId = new Guid(User.FindFirstValue("user_id") ?? 
                                      throw new InvalidOperationException("User ID not found"));
                var response = await _cartService.GetCartAsync(userId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<CartDto>.Error(ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<CartDto>>> AddToCart([FromBody] AddToCartRequest request)
        {
            try
            {
                var userId = new Guid(User.FindFirstValue("user_id"));
                var response = await _cartService.AddToCartAsync(userId, request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<CartDto>.Error(ex.Message));
            }
        }

        [HttpPut]
        public async Task<ActionResult<ApiResponse<CartDto>>> UpdateCartItem([FromBody] UpdateCartItemRequest request)
        {
            try
            {
                var userId = new Guid(User.FindFirstValue("user_id"));
                var response = await _cartService.UpdateCartItemAsync(userId, request);

                if (!response.Succeeded)
                    return NotFound(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<CartDto>.Error(ex.Message));
            }
        }

        [HttpDelete("{itemId}")]
        public async Task<ActionResult<ApiResponse<CartDto>>> RemoveCartItem(Guid itemId)
        {
            try
            {
                var userId = new Guid(User.FindFirstValue("user_id"));
                var response = await _cartService.RemoveCartItemAsync(userId, itemId);

                if (!response.Succeeded)
                    return NotFound(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<CartDto>.Error(ex.Message));
            }
        }

        [HttpDelete]
        public async Task<ActionResult<ApiResponse<bool>>> ClearCart()
        {
            try
            {
                var userId = new Guid(User.FindFirstValue("user_id"));
                var response = await _cartService.ClearCartAsync(userId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>.Error(ex.Message));
            }
        }
    }
}