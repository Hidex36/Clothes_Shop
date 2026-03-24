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

                // Tìm xem sản phẩm này đã có trong giỏ chưa
                var cartItem = cart.Items.FirstOrDefault(i => i.Product.Id == id);
                int currentInCart = cartItem?.Quantity ?? 0;

                // KIỂM TRA TỒN KHO
                if (product.quantity < (currentInCart + quantity))
                {
                    // Nếu không đủ hàng, có thể dùng TempData để báo lỗi ra View
                    TempData["Error"] = $"Rất tiếc, sản phẩm {product.Name} chỉ còn {product.quantity} món.";
                    return RedirectBack();
                }

                cart.AddItem(product, quantity);
                SaveCartToSession(cart);
            }

            return RedirectBack();
        }

        // Hàm bổ trợ để quay lại trang cũ
        private IActionResult RedirectBack()
        {
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

        [HttpPost]
        public IActionResult Plus(int id)
        {
            var product = _context.Products.Find(id);
            var cart = GetCartFromSession();
            var item = cart.Items.FirstOrDefault(x => x.Product.Id == id);

            if (item != null && product != null)
            {
                if (item.Quantity + 1 <= product.quantity)
                {
                    item.Quantity += 1;
                    SaveCartToSession(cart);
                }
                else
                {
                    TempData["Error"] = "Số lượng trong kho đã đạt giới hạn.";
                }
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