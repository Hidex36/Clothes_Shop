namespace Clothes_shop.Models
{
    public class Categories
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Products> products { get; set; }
    }
}
