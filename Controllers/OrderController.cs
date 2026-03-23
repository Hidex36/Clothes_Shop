using Clothes_shop.Infrastructure;
using Clothes_shop.Models;
using Clothes_shop.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

            if (cart.Items.Count == 0)
            {
                ModelState.AddModelError("", "Giỏ hàng của bạn đang trống!");
            }

            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null)
                {
                    order.UserId = currentUser.Id;
                }
                _orderResponsitory.CreateOrder(order, cart);

                // Xóa giỏ hàng sau khi đặt thành công
                HttpContext.Session.Remove("Cart");
                _cart.ClearCart();

                return RedirectToAction("ThankYou");
            }

            ViewBag.Cart = cart;
            return View(order);
        }

        public IActionResult ThankYou()
        {
            _cart.ClearCart();
            return View();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> OrderList()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Content("không tìm thấy user");
            }

            var order = _orderResponsitory.GetAllOrders()
                .Where(i => i.UserId == currentUser.Id)
                .OrderByDescending(i => i.OrderDate)
                .ToList();
            return View(order);
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> OrderDetail(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Content("không tìm thấy user");
            }


            var order = await _orderResponsitory.GetOrderById(id);


            if (order == null || order.UserId != currentUser.Id)
            {
                return NotFound();
            }

            return View(order);
        }

    }
}