using Clothes_shop.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clothes_shop.ViewComponents
{
    public class CategoryMenuViewComponent:ViewComponent
    {
        public readonly AppDbContext _context;
        public CategoryMenuViewComponent(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(bool isHeader = false)
        {
            var categories = await _context.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();
            ViewBag.IsHeader = isHeader;
            return View(categories);
        }
    }
}
