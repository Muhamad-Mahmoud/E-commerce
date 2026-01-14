    
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTO.Cart
{
    public class UpdateCartItemDto
    {
        [Required]
        public int CartItemId { get; set; }

        [Required]
        [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100")]
        public int Quantity { get; set; }
    }
}
