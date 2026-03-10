using Microsoft.AspNetCore.Mvc;
using Clothes_shop.Data;

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
            var bestSellers = _context.Products
                .OrderByDescending(p => p.quantity)
                .Take(7)
                .ToList();
            return View(bestSellers);
        }
    }
}
