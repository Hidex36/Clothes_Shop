using System.ComponentModel.DataAnnotations.Schema;

namespace Clothes_shop.Models
{
    public class Products
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public string Description { get; set; }
        public int quantity { get; set; } //tong so luong san pham (kho)
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int CategoryId { get; set; }
        public Categories Category { get; set; }
    }
}
