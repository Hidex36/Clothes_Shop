using Clothes_shop.Data;
using Clothes_shop.Models;

namespace Clothes_shop.Repositories
{
    public class OrderResponsitory : IOrderResponsitory
    {
        private readonly AppDbContext _context;

        private Cart _cart;
        public OrderResponsitory(AppDbContext context, Cart cart)
        {
            _context = context;
            _cart = cart;
        }
        public IQueryable<Orders> GetAllOrders()
        {
            return _context.Orders;
        }
        public void CreateOrder(Orders order)
        {
            _context.Orders.Add(order);
            _context.SaveChanges();
        }
    }
}
