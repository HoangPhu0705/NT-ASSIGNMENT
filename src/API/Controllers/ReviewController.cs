using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces.Reviews;
using SharedViewModels.Review;
using SharedViewModels.Shared;
using System.Security.Claims;
using OpenIddict.Validation.AspNetCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/review")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet("product/{productId}")]
        public async Task<ActionResult<ApiResponse<ProductReviewsDto>>> GetProductReviews(Guid productId)
        {
            try
            {
                var response = await _reviewService.GetProductReviewsAsync(productId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<ProductReviewsDto>.Error(ex.Message));
            }
        }

        [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme, Roles = "User")]  
        [HttpGet("user")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ReviewDto>>>> GetUserReviews()
        {
            try
            {
                var userId = new Guid(User.FindFirstValue("user_id") ?? throw new InvalidOperationException());
                var response = await _reviewService.GetUserReviewsAsync(userId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<IEnumerable<ReviewDto>>.Error(ex.Message));
            }
        }

        [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme, Roles = "User")]  
        [HttpPost]
        public async Task<ActionResult<ApiResponse<ReviewDto>>> CreateReview([FromBody] CreateReviewRequest request)
        {
            try
            {
                var userId = new Guid(User.FindFirstValue("user_id") ?? throw new InvalidOperationException());
                var response = await _reviewService.CreateReviewAsync(userId, request);
                
                if (!response.Succeeded)
                    return BadRequest(response);
                    
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<ReviewDto>.Error(ex.Message));
            }
        }

        [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme,Roles = "User")]  
        [HttpPut]
        public async Task<ActionResult<ApiResponse<ReviewDto>>> UpdateReview([FromBody] UpdateReviewRequest request)
        {
            try
            {
                var userId = new Guid(User.FindFirstValue("user_id") ?? throw new InvalidOperationException());
                var response = await _reviewService.UpdateReviewAsync(userId, request);
                
                if (!response.Succeeded)
                    return BadRequest(response);
                    
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<ReviewDto>.Error(ex.Message));
            }
        }

        [Authorize(
        AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme,
        Roles = "User")]  
        [HttpDelete("{reviewId}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteReview(Guid reviewId)
        {
            try
            {
                var userId = new Guid(User.FindFirstValue("user_id") ?? throw new InvalidOperationException());
                var response = await _reviewService.DeleteReviewAsync(userId, reviewId);
                
                if (!response.Succeeded)
                    return BadRequest(response);
                    
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>.Error(ex.Message));
            }
        }

        [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme, Roles = "User")]  
        [HttpGet("user/has-reviewed/{productId}")]
        public async Task<ActionResult<ApiResponse<bool>>> HasUserReviewedProduct(Guid productId)
        {
            try
            {
                var userId = new Guid(User.FindFirstValue("user_id") ?? throw new InvalidOperationException());
                var response = await _reviewService.UserHasReviewedProductAsync(userId, productId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>.Error(ex.Message));
            }
        }
    }
}