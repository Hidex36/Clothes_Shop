namespace Clothes_shop.Models
{
    public enum OrderStatus 
    {
        Pending = 0,    // Đang chờ xử lý
        Confirmed = 1,  // Đã xác nhận
        Shipping = 2,   // Đang giao hàng
        Completed = 3,  // Đã hoàn thành
        Cancelled = 4   // Đã hủy
    }
}
