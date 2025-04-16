using Microsoft.AspNetCore.Mvc;
using SharedViewModels.Auth;

namespace CustomerSite.Controllers
{
    public class AuthController : Controller
    {
        [HttpGet]
        public IActionResult CreateAccount()
        {
            var model = new RegisterUserRequest();
            ViewData["Title"] = "Create Account";
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            var model = new LoginUserRequest();
            ViewData["Title"] = "Login";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateAccount(RegisterUserRequest registerUserRequest)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("Login");
            }

            // Pass validation errors to ViewData
            ViewData["FirstNameError"] = ModelState["FirstName"]?.Errors.FirstOrDefault()?.ErrorMessage;
            ViewData["LastNameError"] = ModelState["LastName"]?.Errors.FirstOrDefault()?.ErrorMessage;
            ViewData["EmailError"] = ModelState["Email"]?.Errors.FirstOrDefault()?.ErrorMessage;
            ViewData["PasswordError"] = ModelState["Password"]?.Errors.FirstOrDefault()?.ErrorMessage;

            ViewData["Title"] = "Create Account";
            return View(registerUserRequest);
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