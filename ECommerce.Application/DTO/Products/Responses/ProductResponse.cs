using ECommerce.Domain.Enums;

namespace ECommerce.Application.DTO.Products.Responses
{
    public class ProductResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public ProductStatus Status { get; set; }

        // Main/Lowest price to show in listing
        public decimal Price { get; set; }
        public string? MainImageUrl { get; set; }
    }
}
