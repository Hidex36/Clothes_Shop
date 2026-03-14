using Clothes_shop.Infrastructure;

namespace Clothes_shop.Models
{
    public class Cart
    {
        public class CartItem
        {
            public int CartId { get; set; }
            public Products Product { get; set; }
            public int Quantity { get; set; }
        }

        public List<CartItem> Items { get; set; } = new List<CartItem>();

        public static Cart GetCart(IServiceProvider services)
        {
            ISession? session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext?.Session;

            Cart cart = session?.GetJson<Cart>("Cart") ?? new Cart();
            return cart;
        }
        public virtual IEnumerable<CartItem> GetAllItems()
        {
            return Items;
        }

        public virtual void AddItem(Products product, int quantity)
        {

            CartItem item = Items.FirstOrDefault(i => i.Product.Id == product.Id);
            if (item == null)
            {
                Items.Add(new CartItem
                {
                    Product = product,
                    Quantity = quantity
                });
            }
            else
            {
                item.Quantity += quantity;
            }
        }
        public virtual void ClearCart()
        {
            Items.Clear();
        }
        public virtual void RemoveItem(int productId)
        {
            Items.RemoveAll(i => i.Product.Id == productId);
        }

        public int ComputeTotalQuantity() => Items.Sum(i => i.Quantity);

        public decimal ComputeTotalValue()
        {
            return Items.Sum(i => (decimal)i.Product.Price * i.Quantity);
        }
    }
}
