namespace ECommerce.Application.DTO
{
    public class ProductVariantDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string VariantName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string? Color { get; set; }
        public string? Size { get; set; }
        public bool IsActive { get; set; }
    }
}
