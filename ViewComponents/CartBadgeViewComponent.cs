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
                var cart = JsonConvert.DeserializeObject<Cart>(jsonCart);
                totalItems = cart.Items.Sum(i => i.Quantity);
            }

            return View(totalItems); // Truyền số lượng này sang View của Component
        }
    }
}
