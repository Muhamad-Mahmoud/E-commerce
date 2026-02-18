
namespace ECommerce.Domain.Entities
{
    public class Wishlist : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public ICollection<WishlistItem> Items { get; set; } = new List<WishlistItem>();
    }
}
