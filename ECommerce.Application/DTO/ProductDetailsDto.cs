using ECommerce.Domain.Enums;

namespace ECommerce.Application.DTO
{
    public class ProductDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public ProductStatus Status { get; set; }
        
        public List<ProductVariantDto> Variants { get; set; } = new();
        public List<string> Images { get; set; } = new();
    }
}
