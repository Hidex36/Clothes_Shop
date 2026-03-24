using Clothes_shop.Repositories;
using Microsoft.AspNetCore.Mvc;
using Clothes_shop.Models;
using Microsoft.EntityFrameworkCore;

namespace Clothes_shop.Areas.Admin.ViewComponents
{
    public class OrderBadgeViewComponent : ViewComponent
    {
        private readonly IOrderResponsitory _orderRepository;
        public OrderBadgeViewComponent(IOrderResponsitory orderRepository)
        {
            _orderRepository = orderRepository;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Lấy danh sách đơn hàng
            var ordersQuery = _orderRepository.GetAllOrders()
                                .Where(o => o.Status == OrderStatus.Pending)
                                .OrderBy(o => o.OrderDate);

            var newOrders = await ordersQuery.Take(5).ToListAsync();

            // Đếm tổng số đơn
            ViewBag.Count = await ordersQuery.CountAsync();

            return View(newOrders);
        }
    }
}
