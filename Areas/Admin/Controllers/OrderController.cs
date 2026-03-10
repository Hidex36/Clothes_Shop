using Microsoft.AspNetCore.Mvc;

namespace Clothes_shop.Areas.Admin.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
