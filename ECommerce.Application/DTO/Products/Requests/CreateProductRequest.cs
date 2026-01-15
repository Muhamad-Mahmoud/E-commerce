using System.ComponentModel.DataAnnotations;
using ECommerce.Domain.Enums;

namespace ECommerce.Application.DTO.Products.Requests
{
    public class CreateProductRequest
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public ProductStatus Status { get; set; } = ProductStatus.Draft;

        public List<CreateVariantRequest> Variants { get; set; } = new();
    }

    public class CreateVariantRequest
    {
        [Required]
        public string SKU { get; set; } = string.Empty;

        [Required]
        public string VariantName { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        public string? Color { get; set; }
        public string? Size { get; set; }
    }
}
