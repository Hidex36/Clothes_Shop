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
        public void CreateOrder(Orders order, Cart cart)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    order.OrderDate = DateTime.Now;
                    order.OrderDetails = new List<OrderDetails>();

                    foreach (var item in cart.Items)
                    {
                        // 1. Lấy sản phẩm từ Database (để có số lượng mới nhất)
                        var product = _context.Products.Find(item.Product.Id);

                        if (product == null)
                        {
                            throw new Exception($"Sản phẩm mã {item.Product.Id} không tồn tại.");
                        }

                        if (product.quantity < item.Quantity)
                        {
                            throw new Exception($"Sản phẩm {product.Name} hiện chỉ còn {product.quantity} món. Vui lòng cập nhật lại giỏ hàng.");
                        }

                        product.quantity -= item.Quantity;

                        // 4. Tạo chi tiết đơn hàng
                        var orderDetail = new OrderDetails
                        {
                            ProductId = item.Product.Id,
                            Quantity = item.Quantity,
                            UnitPrice = product.Price // Lấy giá từ DB cho chính xác
                        };
                        order.OrderDetails.Add(orderDetail);

                        _context.Products.Update(product);
                    }

                    _context.Orders.Add(order);
                    _context.SaveChanges();

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw; 
                }
            }
        }

        public async Task<Orders?> GetOrderById(int id)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.Id == id); 
        }

        public void UpdateStatus(int orderId, OrderStatus status)
        {
            var order = _context.Orders.Find(orderId);
            if (order != null)
            {
                order.Status = status;
                _context.SaveChanges();
            }
        }
    }
}
