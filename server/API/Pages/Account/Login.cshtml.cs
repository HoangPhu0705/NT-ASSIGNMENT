using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Domain.Entities;
using SharedViewModels.Auth;
using System.Threading.Tasks;

namespace API.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public LoginModel(SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [BindProperty]
        public LoginUserRequest Input { get; set; }

        public void OnGet(string returnUrl = null)
        {   
            Console.WriteLine("get vo ne");
            Input = new LoginUserRequest
            {
                ReturnUrl = returnUrl
            };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Console.WriteLine("post vo ne");
            if (!ModelState.IsValid)
            {
                ViewData["Error"] = "Please correct the errors in the form.";
                return Page();
            }
            
            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                ViewData["Error"] = "Invalid email or password.";
                return Page();
            }
            
            var result = await _signInManager.PasswordSignInAsync(user, Input.Password, isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                if (!await _signInManager.CanSignInAsync(user))
                {
                    ViewData["Error"] = "This account is not allowed to sign in.";
                    await _signInManager.SignOutAsync();
                    return Page();
                }
            
                return LocalRedirect(Input.ReturnUrl ?? "/");
            }

            ViewData["Error"] = "Invalid email or password.";
            return Page();
        }
    }
}