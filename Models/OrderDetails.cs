using System.ComponentModel.DataAnnotations.Schema;

namespace Clothes_shop.Models
{
    public class OrderDetails
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Orders Order { get; set; }

        public int ProductId { get; set; }
        public Products Product { get; set; }
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }
    }
}
