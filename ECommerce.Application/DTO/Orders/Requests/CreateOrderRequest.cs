using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTO.Orders.Requests
{
    public class CreateOrderRequest
    {
        [Required(ErrorMessage = "Shipping address ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Shipping address ID must be a valid positive number")]
        public int ShippingAddressId { get; set; }
    }
}
