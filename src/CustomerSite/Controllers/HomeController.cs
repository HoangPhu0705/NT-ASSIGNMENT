using CustomerSite.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CustomerSite.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    
        public IActionResult Shop()
        {
            return View();
        }
        

    }
}
