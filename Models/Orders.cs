using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Clothes_shop.Models
{
    public class Orders
    {
        [BindNever]
        public int Id { get; set; }
        public string Name{ get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingPhone { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;

        public int Status { get; set; }
        [BindNever]
        public ICollection<OrderDetails> OrderDetails { get; set; }
    }
}
