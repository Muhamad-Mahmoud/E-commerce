using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTO.Cart.Requests
{
    public class UpdateCartItemRequest
    {
        [Required(ErrorMessage = "Cart Item ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Cart Item ID must be a valid positive number")]
        public int CartItemId { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100")]
        public int Quantity { get; set; }
    }
}
