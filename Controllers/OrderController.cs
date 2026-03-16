using Clothes_shop.Infrastructure;
using Clothes_shop.Models;
using Clothes_shop.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Clothes_shop.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderResponsitory _orderResponsitory;
        private readonly Cart _cart;
        private readonly UserManager<Users> _userManager;

        public OrderController(IOrderResponsitory orderResponsitory, Cart cart, UserManager<Users> userManager)
        {
            _orderResponsitory = orderResponsitory;
            _cart = cart;
            _userManager = userManager;
        }
        [Authorize]
        [HttpGet]
        public IActionResult CheckOut()
        {
            var cart = HttpContext.Session.GetJson<Cart>("Cart") ?? new Cart();
            ViewBag.Cart = cart;
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CheckOut(Orders order)
        {
            var cart = HttpContext.Session.GetJson<Cart>("Cart") ?? new Cart();

            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null)
                {
                    // Bây giờ cả hai đều là kiểu int, gán trực tiếp rất mượt
                    order.UserId = currentUser.Id;
                }
                if (cart.Items.Count == 0)
                {
                    ModelState.AddModelError("", "Giỏ hàng của bạn đang trống!");
                    ViewBag.Cart = cart;
                    return View(order);
                }

                if (ModelState.IsValid)
                {
                    order.OrderDetails = cart.Items.Select(i => new OrderDetails
                    {
                        ProductId = i.Product.Id,
                        Quantity = i.Quantity,
                        UnitPrice = i.Product.Price
                    }).ToList();

                    _orderResponsitory.CreateOrder(order);

                    HttpContext.Session.Remove("Cart");

                    return RedirectToAction("ThankYou");
                }
            }

            ViewBag.Cart = cart;
            return View(order);
        }

        public IActionResult ThankYou()
        {
            _cart.ClearCart();
            return View();
        }
    }
}
