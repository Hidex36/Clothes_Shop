using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Clothes_shop.Models
{
    public class Orders
    {
        [BindNever]
        public int Id { get; set; }
        [Required(ErrorMessage ="Hãy nhập tên của bạn")]
        public string Name{ get; set; }
        [Required(ErrorMessage = "Hãy nhập địa chỉ của bạn")]
        public string ShippingAddress { get; set; }
        [Required(ErrorMessage = "Hãy nhập số điẹn thoại của bạn")]
        public string ShippingPhone { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public int? UserId { get; set; }
        [ValidateNever]
        [ForeignKey("UserId")]
        public Users User { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        [ValidateNever]
        public ICollection<OrderDetails> OrderDetails { get; set; }
    }
}
