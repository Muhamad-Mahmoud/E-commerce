namespace ECommerce.Domain.Entities
{
    public class ProductVariant
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public string SKU { get; set; }
        public string VariantName { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string? Color { get; set; }
        public string? Size { get; set; }
        public bool IsActive { get; set; }

        public ICollection<ProductVariantImage> Images { get; set; }
        public ICollection<ShoppingCartItem> CartItems { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
