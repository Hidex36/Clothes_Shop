using Clothes_shop.Models;

namespace Clothes_shop.Repositories
{
    public interface IOrderResponsitory
    {
        void CreateOrder(Orders order,Cart cart);
        IQueryable<Orders> GetAllOrders();
        Task<Orders?> GetOrderById(int id);

        void UpdateStatus(int orderId, OrderStatus status);
    }
}