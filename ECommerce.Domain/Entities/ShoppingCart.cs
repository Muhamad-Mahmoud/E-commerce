namespace ECommerce.Domain.Entities
{
    public class ShoppingCart : BaseEntity
    {
        public string UserId { get; internal set; } = string.Empty;
        public DateTime CreatedAt { get; internal set; }
        public DateTime UpdatedAt { get; internal set; }

        private readonly List<ShoppingCartItem> _items = new();
        public virtual ICollection<ShoppingCartItem> Items 
        { 
            get => _items; 
            internal set 
            {
                _items.Clear();
                if (value != null) _items.AddRange(value);
            }
        }

        internal ShoppingCart() { }

        public ShoppingCart(string userId)
        {
            UserId = userId;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddOrUpdateItem(int productVariantId, int quantity)
        {
            var existingItem = _items.FirstOrDefault(i => i.ProductVariantId == productVariantId);
            if (existingItem != null)
            {
                existingItem.UpdateQuantity(existingItem.Quantity + quantity);
            }
            else
            {
                _items.Add(new ShoppingCartItem(Id, productVariantId, quantity));
            }
            UpdatedAt = DateTime.UtcNow;
        }

        public void RemoveItem(int productVariantId)
        {
            var item = _items.FirstOrDefault(i => i.ProductVariantId == productVariantId);
            if (item != null)
            {
                _items.Remove(item);
                UpdatedAt = DateTime.UtcNow;
            }
        }

        public void Clear()
        {
            _items.Clear();
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
