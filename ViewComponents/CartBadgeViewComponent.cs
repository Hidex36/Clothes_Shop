using Clothes_shop.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Clothes_shop.ViewComponents
{
    public class CartBadgeViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            // Lấy chuỗi Json từ Session
            var jsonCart = HttpContext.Session.GetString("Cart");
            int totalItems = 0;

            if (!string.IsNullOrEmpty(jsonCart))
            {
                // Giải mã chuỗi thành đối tượng Cart
                var cart = JsonConvert.DeserializeObject<Cart>(jsonCart);
                // Tính tổng số lượng (Sử dụng hàm Sum hoặc hàm ComputeTotalQuantity bạn đã viết)
                totalItems = cart.Items.Sum(i => i.Quantity);
            }

            return View(totalItems); // Truyền số lượng này sang View của Component
        }
    }
}
