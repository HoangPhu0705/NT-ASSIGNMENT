using CustomerSite.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using CustomerSite.Services;
using Microsoft.AspNetCore.Authentication;

namespace CustomerSite.Controllers
{
    public class HomeController : Controller
    {
        private readonly CategoryService _categoryService;

        public HomeController(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        public IActionResult Index()
        {   
            
            return View(User);
        }
    }
}
