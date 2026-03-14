using Clothes_shop.Models;

namespace Clothes_shop.Repositories
{
    public interface IOrderResponsitory
    {
        void CreateOrder(Orders order);
        IQueryable<Orders> GetAllOrders();
    }
}