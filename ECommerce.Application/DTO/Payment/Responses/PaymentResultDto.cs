namespace ECommerce.Application.DTO.Payment.Responses
{
    public class PaymentResultDto
    {
        public string? SessionId { get; set; }
        public string? Url { get; set; }
        public string? PaymentStatus { get; set; }
    }
}
