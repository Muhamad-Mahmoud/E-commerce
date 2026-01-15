using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTO.Cart.Requests
{
    public class AddToCartRequest
    {
        [Required(ErrorMessage = "Product Variant ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Product Variant ID must be a valid positive number")]
        public int ProductVariantId { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100")]
        public int Quantity { get; set; }
    }
}
