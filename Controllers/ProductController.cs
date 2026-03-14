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
            var product = _context.Products.Include(p => p.Category).ToList();
            return View(product);
        }

        [HttpGet]
        public IActionResult ProductByCategory(int categoryId) 
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == categoryId);

            if (category == null)
            {
                // Nếu vào đây nghĩa là ID truyền lên không khớp với bất kỳ dòng nào trong bảng Categories
                return NotFound();
            }

            // 2. Lấy danh sách sản phẩm
            var products = _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == categoryId)
                .ToList();

            // 3. Đổ dữ liệu vào ViewModel (Luôn trả về Model, dù list products rỗng hay có)
            var result = new ProductByCategoryViewModel
            {
                CategoryId = category.Id,
                CategoryName = category.Name, // Kiểm tra lại Model của bạn là .Name hay .CategoryName
                Products = products ?? new List<Products>()
            };

            return View(result);
        }
    }
}
