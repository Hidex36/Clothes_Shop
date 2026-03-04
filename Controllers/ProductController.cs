using Microsoft.AspNetCore.Mvc;

namespace Clothes_shop.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
