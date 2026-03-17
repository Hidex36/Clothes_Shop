using Clothes_shop.Data;
using Clothes_shop.Models;
using Microsoft.EntityFrameworkCore;

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
            return _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product);
        }
        public void CreateOrder(Orders order,Cart cart)
        {
            order.OrderDetails = cart.Items.Select(i => new OrderDetails
            {
                ProductId = i.Product.Id,
                Quantity = i.Quantity,
                UnitPrice = i.Product.Price
            }).ToList();

            order.OrderDate = DateTime.Now;
            _context.Orders.Add(order);
            _context.SaveChanges();
        }

        public async Task<Orders?> GetOrderById(int id)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.Id == id); 
        }
    }
}
