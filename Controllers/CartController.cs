using Microsoft.AspNetCore.Mvc;

namespace Clothes_shop.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
