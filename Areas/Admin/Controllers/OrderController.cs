using Clothes_shop.Models;
using Clothes_shop.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clothes_shop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, Staff")]
    public class OrderController : Controller
    {
        private readonly IOrderResponsitory _orderRepo;

        public OrderController(IOrderResponsitory orderRepo)
        {
            _orderRepo = orderRepo;
        }

        // Lấy danh sách đơn hàng đang chờ xử lý
        public IActionResult Index()
        {
            var pendingOrders = _orderRepo.GetAllOrders()
                                .Include(o => o.OrderDetails)
                                .OrderBy(o => o.OrderDate)
                                .ToList();
            return View(pendingOrders);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var order = await _orderRepo.GetOrderById(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // Xử lý xác nhận đơn hàng
        [HttpPost]
        public IActionResult Confirm(int id)
        {
            try
            {
                _orderRepo.UpdateStatus(id, OrderStatus.Confirmed);
                TempData["Success"] = "Đã xác nhận đơn hàng thành công!";
            }
            catch (Exception)
            {
                TempData["Error"] = "Có lỗi xảy ra khi xác nhận đơn hàng.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

