using Clothes_shop.Data;
using Clothes_shop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Clothes_shop.Models.ViewModels;

namespace Clothes_shop.Controllers
{
    public class ProductController : Controller
    {
        public readonly AppDbContext _context;
        public ProductController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ProductByCategory(int id) 
        {
            var products = _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == id) 
                .ToList();

            if (!products.Any())
            {
                var category = _context.Categories.Find(id);
                return View(new ProductByCategoryViewModel
                {
                    CategoryId = id,
                    CategoryName = category?.Name ?? "Danh mục không tồn tại",
                    Products = new List<Products>()
                });
            }

            // 3. Nếu có sản phẩm
            var result = new ProductByCategoryViewModel
            {
                CategoryId = id,
                CategoryName = products.First().Category.Name,
                Products = products
            };

            return View(result);
        }
    }
}
