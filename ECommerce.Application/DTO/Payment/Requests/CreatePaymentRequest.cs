using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTO.Payment.Requests
{
    public class CreatePaymentRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Order ID must be a valid positive number")]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Order number is required")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Order number must be between 1 and 50 characters")]
        public string OrderNumber { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Currency is required")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Currency must be a 3-letter code")]
        public string Currency { get; set; } = "usd";

        [Required(ErrorMessage = "Success URL is required")]
        [Url(ErrorMessage = "Success URL must be a valid URL")]
        public string SuccessUrl { get; set; } = string.Empty;

        [Required(ErrorMessage = "Cancel URL is required")]
        [Url(ErrorMessage = "Cancel URL must be a valid URL")]
        public string CancelUrl { get; set; } = string.Empty;
    }
}
