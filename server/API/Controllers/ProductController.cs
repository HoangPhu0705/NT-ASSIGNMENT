using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class ProductController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}