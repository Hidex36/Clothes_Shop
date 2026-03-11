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
                .Where(p => p.CategoryId == CategoryId)
                .ToList();
            if (!products.Any())
            {
                return View(new ProductByCategoryViewModel
                {
                    CategoryId = CategoryId,
                    CategoryName = "Không tìm thấy sản phẩm",
                    Products = new List<Products>()

                });
            }
            var Result = (new ProductByCategoryViewModel
            {
                CategoryId = CategoryId,
                CategoryName = products.First().Category.Name,
                Products = products

            });
            return View(Result);
        }
}
