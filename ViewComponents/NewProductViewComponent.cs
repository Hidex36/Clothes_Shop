using Microsoft.AspNetCore.Mvc;
using Clothes_shop.Data;

namespace Clothes_shop.ViewComponents
{
    public class NewProductViewComponent: ViewComponent
    {
        public readonly AppDbContext _context;
        public NewProductViewComponent(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var products = _context.Products
                .OrderByDescending(p => p.CreatedAt)
                .Take(7)
                .ToList();
            return View(products);
        }
    }
}
