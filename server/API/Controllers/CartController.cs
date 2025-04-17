using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class CartController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}