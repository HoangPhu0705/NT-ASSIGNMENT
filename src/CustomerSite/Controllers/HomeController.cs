using CustomerSite.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Authentication;

namespace CustomerSite.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            
            return View(User);
        }
    
        public  async Task<IActionResult> Shop()
        {
            return View();
        }
        

    }
}
