using Microsoft.AspNetCore.Mvc;
using Clothes_shop.Data;
using Microsoft.EntityFrameworkCore;
using Clothes_shop.Models;

namespace Clothes_shop.ViewComponents
{
    public class BestSellerViewComponent : ViewComponent
    {
        public readonly AppDbContext _context;
        public BestSellerViewComponent(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Bước 1: Tìm ID của các sản phẩm bán chạy nhất từ bảng OrderDetails
            var bestSellerIds = await _context.OrderDetails
                .Include(od => od.Order)
                .Where(od => od.Order.Status == OrderStatus.Confirmed)
                .GroupBy(od => od.ProductId) // Nhóm theo mã sản phẩm
                .Select(group => new
                {
                    ProductId = group.Key,
                    TotalSold = group.Sum(od => od.Quantity) // Tổng số lượng đã bán
                })
                .OrderByDescending(x => x.TotalSold) // Thằng nào bán nhiều nhất lên đầu
                .Take(7) 
                .Select(x => x.ProductId)
                .ToListAsync();

            // Bước 2: Lấy thông tin chi tiết của các sản phẩm đó
            var bestSellers = await _context.Products
                .Where(p => bestSellerIds.Contains(p.Id))
                .ToListAsync();

            // Sắp xếp lại danh sách sản phẩm theo đúng thứ tự ID đã tìm ở Bước 1
            bestSellers = bestSellers.OrderBy(p => bestSellerIds.IndexOf(p.Id)).ToList();

            return View(bestSellers);
        }
    }
}
