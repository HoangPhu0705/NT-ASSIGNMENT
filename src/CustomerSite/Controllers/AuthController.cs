using CustomerSite.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using SharedViewModels.Auth;
using System.Security.Claims;
using System.Text.Json;

namespace CustomerSite.Controllers
{
    [Route("auth")]
    public class AuthController : Controller
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        [Route("login")]
        public async Task<IActionResult> Login(string returnUrl = "/")
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            ViewData["Title"] = "Sign In";
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = returnUrl,
            }, 
            OpenIdConnectDefaults.AuthenticationScheme);
        }
        
        
        [HttpGet]
        [Route("create-account")]
        public IActionResult CreateAccount()
        {
            var model = new RegisterUserRequest();
            ViewData["Title"] = "Create Account";
            return View(model);
        }
        
        
        [HttpGet]
        [Route("logout")]
        public IActionResult Logout()
        {
            return SignOut(
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action("Index", "Home")
                },
                CookieAuthenticationDefaults.AuthenticationScheme,
                OpenIdConnectDefaults.AuthenticationScheme);
        }

        [HttpGet]
        [Route("profile")]
        public IActionResult Profile()
        {   
            string accessToken = HttpContext.GetTokenAsync(OpenIdConnectDefaults.AuthenticationScheme, "access_token").Result;
            var testModel = new TestModel
            {
                AccessToken = accessToken,
                Claims = User.Claims.ToList()
            };
            return View(testModel);
        }

        [HttpGet]
        [Route("confirm-email")]
        public IActionResult ConfirmEmail()
        {
            ViewData["Title"] = "Confirm Your Email";
            ViewData["Email"] = TempData["Email"]?.ToString();
            return View();
        }

        [HttpGet]
        [Route("confirm-email-callback")]
        public IActionResult ConfirmEmailCallback(string userId, string token)
        {
            return View();
        }

        [HttpPost]
        [Route("create-account")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAccount(RegisterUserRequest registerUserRequest)
        {
            if (!ModelState.IsValid)
            {
                ViewData["FirstNameError"] = ModelState["FirstName"]?.Errors.FirstOrDefault()?.ErrorMessage;
                ViewData["LastNameError"] = ModelState["LastName"]?.Errors.FirstOrDefault()?.ErrorMessage;
                ViewData["EmailError"] = ModelState["Email"]?.Errors.FirstOrDefault()?.ErrorMessage;
                ViewData["PasswordError"] = ModelState["Password"]?.Errors.FirstOrDefault()?.ErrorMessage;
                ViewData["Title"] = "Create Account";
                return View(registerUserRequest);
            }

            var response = await _authService.RegisterAsync(registerUserRequest);   
            if (response.Succeeded)
            {
                TempData["Email"] = registerUserRequest.Email;
                return RedirectToAction("ConfirmEmail");
            }

            ViewData["FirstNameError"] = ModelState["FirstName"]?.Errors.FirstOrDefault()?.ErrorMessage;
            ViewData["LastNameError"] = ModelState["LastName"]?.Errors.FirstOrDefault()?.ErrorMessage;
            ViewData["EmailError"] = ModelState["Email"]?.Errors.FirstOrDefault()?.ErrorMessage;
            ViewData["PasswordError"] = ModelState["Password"]?.Errors.FirstOrDefault()?.ErrorMessage;
            ViewData["Title"] = "Create Account";
            return View(registerUserRequest);
        }

        [HttpPost]
        [Route("resend-confirmation-email")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendConfirmationEmail(string email)
        {
            var response = await _authService.ResendConfirmationEmailAsync(email);
            if (response.Succeeded)
            {
                TempData["Email"] = email;
                return RedirectToAction("ConfirmEmail");
            }

            ModelState.AddModelError("Email", response.Message);
            return RedirectToAction("ConfirmEmail");
        }
        
        [HttpPost]
        [Route("oauth/{provider}")]
        [ValidateAntiForgeryToken]
        public IActionResult OAuthLogin(string provider, string returnUrl)
        {
            if (provider.ToLower() != "google")
            {
                return BadRequest("Unsupported provider.");
            }

            var properties = new AuthenticationProperties
            {
                RedirectUri = returnUrl ?? Url.Action("Index", "Home")
            };
            return Challenge(properties, "Google");
        }
    }
    
    
    public class TestModel
    {
        public string AccessToken { get; set; }
        public List<Claim> Claims { get; set; }
    }
    
}