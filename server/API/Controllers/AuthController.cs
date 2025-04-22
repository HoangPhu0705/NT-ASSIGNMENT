using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using SharedViewModels.Auth;
using SharedViewModels.Shared;
using System.Security.Claims;
using Domain.Entities;
using Application.Interfaces.Auth;

namespace API.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public AuthController(
            IAuthService authService,
            SignInManager<User> signInManager,
            UserManager<User> userManager)
        {
            _authService = authService;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost("~/api/auth/register")]
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

        [HttpPost("~/api/auth/resend-confirmation-email")]
        public async Task<IActionResult> ResendConfirmationEmail([FromBody] string email)
        {
            try
            {
                var response = await _authService.ResendConfirmationEmailAsync(email);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.Error(ex.Message));
            }
        }

        [HttpGet("~/api/auth/confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            try
            {
                var response = await _authService.ConfirmEmailAsync(userId, token);
                if (response.Succeeded)
                {
                    return Redirect("https://localhost:7001/auth/confirm-email-callback");
                }
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.Error(ex.Message));
            }
        }

        [HttpPost("~/connect/token"), Produces("application/json")]
        public async Task<IActionResult> Exchange()
        {
            var request = HttpContext.GetOpenIddictServerRequest() ??
                          throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");
            ClaimsPrincipal principal;

            if (request.IsAuthorizationCodeGrantType() || request.IsRefreshTokenGrantType())
            {
                principal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal;
                var user = await _userManager.GetUserAsync(principal);
                if (user is null)
                    return ForbidWithError(OpenIddictConstants.Errors.ExpiredToken, "The token is no longer valid.");
                if (!await _signInManager.CanSignInAsync(user))
                    return ForbidWithError(OpenIddictConstants.Errors.AccessDenied, "The user is no longer allowed to sign in.");
                principal = await CreateClaimsPrincipalAsync(user);
            }
            else
            {
                throw new InvalidOperationException("The specified grant type is not supported.");
            }

            var requestedScopes = request.GetScopes();
            principal.SetScopes(requestedScopes);
            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        [HttpPost("~/api/auth/{provider}")]
        public async Task<IActionResult> OAuthLogin(string provider, [FromBody] OAuthRequest request)
        {
            try
            {
                var user = await _authService.CreateOrGetUserFromOAuthAsync(provider, request);
                var principal = await CreateClaimsPrincipalAsync(user);
                return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.Error(ex.Message));
            }
        }

        [HttpGet("~/connect/authorize")]
        [HttpPost("~/connect/authorize")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Authorize()
        {
            var request = HttpContext.GetOpenIddictServerRequest() ??
                          throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

            var result = await HttpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme);

            if (!result.Succeeded)
            {
                return Challenge(
                    authenticationSchemes: IdentityConstants.ApplicationScheme,
                    properties: new AuthenticationProperties
                    {
                        RedirectUri = Request.PathBase + Request.Path + QueryString.Create(
                            Request.HasFormContentType ? Request.Form.ToList() : Request.Query.ToList())
                    });
            }

            var user = await _userManager.GetUserAsync(result.Principal) ??
                       throw new InvalidOperationException("The user details cannot be retrieved.");

            var principal = await CreateClaimsPrincipalAsync(user);
            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        [HttpGet("~/connect/logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return SignOut(
                authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties
                {
                    RedirectUri = "/"
                });
        }

        private async Task<ClaimsPrincipal> CreateClaimsPrincipalAsync(User user)
        {
            var principal = await _signInManager.CreateUserPrincipalAsync(user);
            var identity = principal.Identity as ClaimsIdentity;
            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Any())
            {
                identity.AddClaim(new Claim(OpenIddictConstants.Claims.Role, roles.First()));
            }

            identity.AddClaim(new Claim("user_id", user.Id.ToString()));
            identity.AddClaim(new Claim("first_name", user.FirstName ?? string.Empty));
            identity.AddClaim(new Claim("last_name", user.LastName ?? string.Empty));

            return principal;
        }

        private IActionResult ForbidWithError(string error, string description)
        {
            var properties = new AuthenticationProperties(new Dictionary<string, string?>
            {
                [OpenIddictServerAspNetCoreConstants.Properties.Error] = error,
                [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = description
            });
            return Forbid(properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
    }
}