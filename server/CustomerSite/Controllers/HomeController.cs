using CustomerSite.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CustomerSite.Controllers
{
    public class HomeController : Controller
    {

        public IActionResult Index()
        {
            var model = new HomeIndexViewModel
            {
                Email = User.FindFirst("email")?.Value
            };
            return View(model);
        }
        
        
        public class HomeIndexViewModel
        {
            public string Email { get; set; }
        }
    }
}
