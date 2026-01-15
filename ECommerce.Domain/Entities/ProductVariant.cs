namespace ECommerce.Domain.Entities
{
    public class ProductVariant : BaseEntity
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public string SKU { get; set; }
        public string VariantName { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string? Color { get; set; }
        public string? Size { get; set; }
        public bool IsActive { get; set; }

        public void DeductStock(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity to deduct must be greater than zero.");

            if (StockQuantity < quantity)
                throw new InvalidOperationException($"Insufficient stock. Available: {StockQuantity}, Requested: {quantity}");

            StockQuantity -= quantity;
        }

        public void RestoreStock(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity to restore must be greater than zero.");

            StockQuantity += quantity;
        }

        public ICollection<ProductVariantImage> Images { get; set; }
        public ICollection<ShoppingCartItem> CartItems { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
