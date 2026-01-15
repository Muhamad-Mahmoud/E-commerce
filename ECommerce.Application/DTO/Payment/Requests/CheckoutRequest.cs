using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTO.Payment.Requests
{
    public class CheckoutRequest
    {
        [Required(ErrorMessage = "Order ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Order ID must be a valid positive number")]
        public int OrderId { get; set; }
    }
}
