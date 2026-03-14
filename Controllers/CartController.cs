using Clothes_shop.Data;
using Clothes_shop.Infrastructure;
using Clothes_shop.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Clothes_shop.Controllers
{
    public class CartController : Controller
    {
        public readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }

        // Hàm tiện ích để lấy giỏ hàng
        private Cart GetCartFromSession()
        {
            return HttpContext.Session.GetJson<Cart>("Cart") ?? new Cart();
        }

        // Hàm tiện ích để lưu giỏ hàng
        private void SaveCartToSession(Cart cart)
        {
            HttpContext.Session.SetJson("Cart", cart);
        }

        public IActionResult Index()
        {
            var cart = GetCartFromSession();
            return View(cart);
        }

        [HttpPost]
        public IActionResult AddToCart(int id, int quantity = 1)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                var cart = GetCartFromSession();
                cart.AddItem(product, quantity);
                SaveCartToSession(cart);
            }

            // Quay lại trang trước đó hoặc về Index nếu không có Referer
            var referer = Request.Headers["Referer"].ToString();
            return string.IsNullOrEmpty(referer) ? RedirectToAction("Index") : Redirect(referer);
        }

        // ĐÂY LÀ HÀM CHO NÚT TRỪ (-)
        [HttpPost]
        public IActionResult Remove(int id)
        {
            var cart = GetCartFromSession();

            // Tìm sản phẩm trong danh sách Items của Cart
            var item = cart.Items.FirstOrDefault(x => x.Product.Id == id);

            if (item != null)
            {
                if (item.Quantity > 1)
                {
                    // Nếu nhiều hơn 1 thì giảm đi 1
                    item.Quantity -= 1;
                }
                else
                {
                    // Nếu chỉ còn 1 thì xóa luôn sản phẩm đó khỏi giỏ
                    cart.Items.Remove(item);
                }
                SaveCartToSession(cart);
            }

            return RedirectToAction("Index");
        }

        // HÀM CHO NÚT THÙNG RÁC (Xóa hẳn dòng sản phẩm)
        [HttpPost]
        public IActionResult RemoveLine(int id)
        {
            var cart = GetCartFromSession();
            // Sử dụng hàm RemoveItem có sẵn trong Model Cart của bạn (nếu có)
            // Hoặc xóa trực tiếp bằng Linq:
            var item = cart.Items.FirstOrDefault(x => x.Product.Id == id);
            if (item != null)
            {
                cart.Items.Remove(item);
                SaveCartToSession(cart);
            }
            return RedirectToAction("Index");
        }

        public IActionResult Clear()
        {
            var cart = GetCartFromSession();
            cart.ClearCart();
            SaveCartToSession(cart);
            return RedirectToAction("Index");
        }
    }
}