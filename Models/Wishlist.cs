namespace Clothes_shop.Models
{
    public class Wishlist
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public Users User { get; set; }

        public int ProductId { get; set; }
        public Products Product { get; set; }
    }
}
