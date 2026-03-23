using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Clothes_shop.Models
{
    public class Users : IdentityUser<int>
    {
        [StringLength(100)]
        public string? FullName { get; set; }
        [StringLength(100)]
        public string? Address { get; set; }

        [StringLength(100)]
        public string? AvatarImageUrl { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? DateOfBirth { get; set; }
        public int? Sex { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual ICollection<Orders> Orders { get; set; } = new List<Orders>();

        public virtual ICollection<WishList> WishLists { get; set; } = new List<WishList>();
    }

}