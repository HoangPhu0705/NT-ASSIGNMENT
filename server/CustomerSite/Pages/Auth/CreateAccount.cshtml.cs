using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SharedViewModels.Auth;

namespace CustomerSite.Pages.Auth;

public class CreateAccount : PageModel
{
    [BindProperty]
    public RegisterUserRequest RegisterUserRequest { get; set; }

    
    public void OnGet()
    {
        RegisterUserRequest = new RegisterUserRequest();
    }
    
    public void OnPost()
    {
    }
}