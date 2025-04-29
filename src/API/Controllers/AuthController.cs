using System.Collections.Immutable;
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
using System.Text.Json;
using Domain.Entities;
using Application.Interfaces.Auth;
using Microsoft.IdentityModel.Tokens;

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

       [HttpPost("~/connect/token"), IgnoreAntiforgeryToken, Produces("application/json")]
        public async Task<IActionResult> Exchange()
        {
            var request = HttpContext.GetOpenIddictServerRequest() ??
                throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

            if (request.IsAuthorizationCodeGrantType() || request.IsRefreshTokenGrantType())
            {
                var result = await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

                // Retrieve the user profile corresponding to the authorization code/refresh token.
                var user = await _userManager.FindByIdAsync(result.Principal.GetClaim(OpenIddictConstants.Claims.Subject));
                if (user is null)
                {
                    return Forbid(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The token is no longer valid."
                        }));
                }

                if (!await _signInManager.CanSignInAsync(user))
                {
                    return Forbid(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The user is no longer allowed to sign in."
                        }));
                }

                var identity = new ClaimsIdentity(
                    result.Principal.Claims,
                    authenticationType: TokenValidationParameters.DefaultAuthenticationType,
                    nameType: OpenIddictConstants.Claims.Name,
                    roleType: OpenIddictConstants.Claims.Role);

                identity.SetClaim(OpenIddictConstants.Claims.Subject, await _userManager.GetUserIdAsync(user))
                        .SetClaim(OpenIddictConstants.Claims.Email, await _userManager.GetEmailAsync(user))
                        .SetClaim(OpenIddictConstants.Claims.Name, await _userManager.GetUserNameAsync(user))
                        .SetClaim(OpenIddictConstants.Claims.PreferredUsername, await _userManager.GetUserNameAsync(user))
                        .SetClaim(OpenIddictConstants.Claims.GivenName, user.FirstName ?? string.Empty) // Add first_name
                        .SetClaim(OpenIddictConstants.Claims.FamilyName, user.LastName ?? string.Empty) // Add last_name
                        .SetClaims(OpenIddictConstants.Claims.Role, (await _userManager.GetRolesAsync(user)).ToImmutableArray());

                // Set scopes and resources
                identity.SetScopes(request.GetScopes());

                identity.SetDestinations(GetDestinations);

                return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            throw new InvalidOperationException("The specified grant type is not supported.");
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

            var requestedScopes = request.GetScopes();

            principal.SetScopes(requestedScopes);

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
        
        [Authorize(AuthenticationSchemes = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)]
        [HttpGet("~/connect/userinfo")]
        public async Task<IActionResult> UserInfo()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidToken,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The specified access token is not valid while getting userinfo huhu."
                    }));
            }
    
            var claims = new Dictionary<string, object>
            {
                [OpenIddictConstants.Claims.Subject] = await _userManager.GetUserIdAsync(user),
                [OpenIddictConstants.Claims.Email] = await _userManager.GetEmailAsync(user),
                [OpenIddictConstants.Claims.Name] = user.UserName,
                ["first_name"] = user.FirstName ?? string.Empty,
                ["last_name"] = user.LastName ?? string.Empty
            };
    
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Any())
            {
                claims[OpenIddictConstants.Claims.Role] = roles.ToArray();
            }
            return Ok(claims);
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
            identity.AddClaim(new Claim("email", user.Email ?? string.Empty));

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
        
        private static IEnumerable<string> GetDestinations(Claim claim)
        {
            switch (claim.Type)
            {
                case OpenIddictConstants.Claims.Subject:
                    yield return OpenIddictConstants.Destinations.AccessToken;
                    yield return OpenIddictConstants.Destinations.IdentityToken;
                    yield break;

                case OpenIddictConstants.Claims.Name:
                case OpenIddictConstants.Claims.PreferredUsername:
                    yield return OpenIddictConstants.Destinations.AccessToken;
                    if (claim.Subject.HasScope(OpenIddictConstants.Permissions.Scopes.Profile))
                        yield return OpenIddictConstants.Destinations.IdentityToken;
                    yield break;

                case OpenIddictConstants.Claims.Email:
                    yield return OpenIddictConstants.Destinations.AccessToken;
                    if (claim.Subject.HasScope(OpenIddictConstants.Permissions.Scopes.Email))
                        yield return OpenIddictConstants.Destinations.IdentityToken;
                    yield break;

                case OpenIddictConstants.Claims.GivenName:
                case OpenIddictConstants.Claims.FamilyName:
                    yield return OpenIddictConstants.Destinations.AccessToken;
                    if (claim.Subject.HasScope(OpenIddictConstants.Permissions.Scopes.Profile))
                        yield return OpenIddictConstants.Destinations.IdentityToken;
                    yield break;

                case OpenIddictConstants.Claims.Role:
                    yield return OpenIddictConstants.Destinations.AccessToken;
                    if (claim.Subject.HasScope(OpenIddictConstants.Permissions.Scopes.Roles))
                        yield return OpenIddictConstants.Destinations.IdentityToken;
                    yield break;

                // Never include sensitive claims like security stamps
                case "AspNet.Identity.SecurityStamp":
                    yield break;

                default:
                    yield return OpenIddictConstants.Destinations.AccessToken;
                    yield break;
            }
        }
    }
}