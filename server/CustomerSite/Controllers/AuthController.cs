using CustomerSite.Services;
using Microsoft.AspNetCore.Mvc;
using SharedViewModels.Auth;

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
        [Route("create-account")]
        public IActionResult CreateAccount()
        {
            var model = new RegisterUserRequest();
            ViewData["Title"] = "Create Account";
            return View(model);
        }

        [HttpGet]
        [Route("login")]
        public IActionResult Login()
        {
            var model = new LoginUserRequest();
            ViewData["Title"] = "Login";
            return View(model);
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
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginUserRequest loginUserRequest)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["EmailError"] = ModelState["Email"]?.Errors.FirstOrDefault()?.ErrorMessage;
            ViewData["PasswordError"] = ModelState["Password"]?.Errors.FirstOrDefault()?.ErrorMessage;

            ViewData["Title"] = "Login";
            return View(loginUserRequest);
        }
    }
}