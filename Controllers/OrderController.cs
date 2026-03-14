using Clothes_shop.Infrastructure;
using Clothes_shop.Models;
using Clothes_shop.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Clothes_shop.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderResponsitory _orderResponsitory;
        private readonly Cart _cart;

        public OrderController(IOrderResponsitory orderResponsitory, Cart cart)
        {
            _orderResponsitory = orderResponsitory;
            _cart = cart;
        }

        [HttpGet]
        public IActionResult CheckOut()
        {
            var cart = HttpContext.Session.GetJson<Cart>("Cart") ?? new Cart();
            ViewBag.Cart = cart;
            return View();
        }

        [HttpPost]
        public IActionResult CheckOut(Orders order)
        {
            var cart = HttpContext.Session.GetJson<Cart>("Cart") ?? new Cart();
            if (_cart.Items.Count == 0)
            {
                ModelState.AddModelError("", "Giỏ hàng của bạn đang trống!");
                return View(order);
            }
            if (ModelState.IsValid)
            {
                _orderResponsitory.CreateOrder(order);
                return RedirectToAction("ThankYou");
            }
            ViewBag.Cart = _cart;
            return View(order);
        }

        public IActionResult ThankYou()
        {
            _cart.ClearCart();
            return View();
        }
    }
}
