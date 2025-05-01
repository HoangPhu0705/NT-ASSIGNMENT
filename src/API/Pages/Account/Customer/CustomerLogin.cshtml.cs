using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Domain.Entities;
using SharedViewModels.Auth;
using System.Threading.Tasks;

namespace API.Pages.Account.Customer
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

        public async Task OnGetAsync(string returnUrl = null)
        {   

            Console.WriteLine("get vo ne");
            Input = new LoginUserRequest
            {
                ReturnUrl = returnUrl
            };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Console.WriteLine("Customer login started");
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
    
            // First check password validity without signing in
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, Input.Password);
            if (isPasswordValid)
            {
                if (!await _signInManager.CanSignInAsync(user))
                {
                    ViewData["Error"] = "This account is not allowed to sign in.";
                    return Page();
                }

                // Create the principal and sign in with CustomerScheme explicitly
                var principal = await _signInManager.CreateUserPrincipalAsync(user);
                await HttpContext.SignInAsync("CustomerScheme", principal);

                Console.WriteLine("customer login success");
                return LocalRedirect(Input.ReturnUrl ?? "/");
            }

            ViewData["Error"] = "Invalid email or password.";
            return Page();
        }
    }
}