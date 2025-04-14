using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SharedViewModels.Auth;

namespace CustomerSite.Pages.Auth
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public LoginUserRequest LoginRequest { get; set; }

        public void OnGet()
        {
            LoginRequest = new LoginUserRequest();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Your login logic here
            // Example: await _authService.LoginAsync(LoginRequest.Email, LoginRequest.Password);

            return RedirectToPage("/Index");
        }
    }
}