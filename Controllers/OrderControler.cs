using Microsoft.AspNetCore.Mvc;

namespace Clothes_shop.Controllers
{
    public class OrderControler : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
