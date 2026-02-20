namespace ECommerce.Domain.Entities
{
    public class ProductVariant : BaseEntity
    {
        public int ProductId { get; internal set; }
        public virtual Product Product { get; internal set; } = null!;

        public string SKU { get; internal set; } = string.Empty;
        public string VariantName { get; internal set; } = string.Empty;
        public decimal Price { get; internal set; }
        public int StockQuantity { get; internal set; }
        public string? Color { get; internal set; }
        public string? Size { get; internal set; }
        public bool IsActive { get; internal set; }

        // EF Core constructor
        internal ProductVariant() { }

        public ProductVariant(string sku, string variantName, decimal price, int stockQuantity, string? color = null, string? size = null)
        {
            SKU = sku;
            VariantName = variantName;
            Price = price;
            StockQuantity = stockQuantity;
            Color = color;
            Size = size;
            IsActive = true;
        }

        public void DeductStock(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity to deduct must be greater than zero.");

            if (StockQuantity < quantity)
                throw new InvalidOperationException($"Insufficient stock for variant {SKU}. Available: {StockQuantity}, Requested: {quantity}");

            StockQuantity -= quantity;
        }

        public void RestoreStock(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity to restore must be greater than zero.");

            StockQuantity += quantity;
        }

        public void UpdatePrice(decimal newPrice)
        {
            if (newPrice < 0)
                throw new ArgumentException("Price cannot be negative.");

            Price = newPrice;
        }

        public void UpdateStock(int newQuantity)
        {
            if (newQuantity < 0)
                throw new ArgumentException("Stock quantity cannot be negative.");

            StockQuantity = newQuantity;
        }

        public virtual ICollection<ProductVariantImage> Images { get; internal set; } = new List<ProductVariantImage>();
        public virtual ICollection<ShoppingCartItem> CartItems { get; internal set; } = new List<ShoppingCartItem>();
        public virtual ICollection<OrderItem> OrderItems { get; internal set; } = new List<OrderItem>();
    }
}
