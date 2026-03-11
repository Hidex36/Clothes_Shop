namespace Clothes_shop.Models.ViewModels
{
    public class ProductByCategoryViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public ICollection<Products> Products { get; set; }
    }
}
