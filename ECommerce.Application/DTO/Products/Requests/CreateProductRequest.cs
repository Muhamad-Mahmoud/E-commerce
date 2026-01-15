using System.ComponentModel.DataAnnotations;
using ECommerce.Domain.Enums;

namespace ECommerce.Application.DTO.Products.Requests
{
    public class CreateProductRequest
    {
        [Required(ErrorMessage = "Product name is required")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Product name must be between 2 and 200 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Category ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Category ID must be a valid positive number")]
        public int CategoryId { get; set; }

        public ProductStatus Status { get; set; } = ProductStatus.Draft;

        [MinLength(1, ErrorMessage = "At least one product variant is required")]
        public List<CreateVariantRequest> Variants { get; set; } = new();
    }

    public class CreateVariantRequest
    {
        [Required(ErrorMessage = "SKU is required")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "SKU must be between 1 and 50 characters")]
        public string SKU { get; set; } = string.Empty;

        [Required(ErrorMessage = "Variant name is required")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Variant name must be between 1 and 100 characters")]
        public string VariantName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 1000000, ErrorMessage = "Price must be between 0.01 and 1,000,000")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Stock quantity is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative")]
        public int StockQuantity { get; set; }

        [StringLength(50, ErrorMessage = "Color cannot exceed 50 characters")]
        public string? Color { get; set; }

        [StringLength(20, ErrorMessage = "Size cannot exceed 20 characters")]
        public string? Size { get; set; }
    }
}
