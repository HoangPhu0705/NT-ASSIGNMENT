using Application.DTOs.Auth;
using Application.DTOs.Shared;
using Application.Interfaces.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;

namespace API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
        {
            try
            {
                var response = await _authService.RegisterAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.Error(ex.Message));
            }
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            try
            {
                var response = await _authService.ConfirmEmailAsync(userId, token);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.Error(ex.Message));
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserRequest request)
        {
            try
            {
                var response = await _authService.LoginAsync(request);
                SetTokenCookie(response.Token);
                return Ok(ApiResponse<AuthResponse>.Success(response, "Login successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.Error(ex.Message));
            }
        }

        [HttpPost("{provider}")]
        public async Task<IActionResult> OAuthLogin(string provider, [FromBody] OAuthRequest request)
        {
            try
            {
                var response = await _authService.HandleOAuthLoginAsync(provider, request);
                SetTokenCookie(response.Token);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.Error(ex.Message));
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("access_token");
            return Ok(ApiResponse<string>.Success("Logged out successfully"));
        }


        private void SetTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true, 
                Secure = true,  
                SameSite = SameSiteMode.Strict, // Prevents CSRF
                Expires = DateTimeOffset.UtcNow.AddMinutes(60) 
            };
            Response.Cookies.Append("access_token", token, cookieOptions);
        }

    }
}
