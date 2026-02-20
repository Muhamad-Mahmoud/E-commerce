namespace ECommerce.Domain.Entities
{
    public class ShoppingCart : BaseEntity
    {
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<ShoppingCartItem> Items { get; set; }
    }
}
