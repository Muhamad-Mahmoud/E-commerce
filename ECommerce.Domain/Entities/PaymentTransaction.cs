using ECommerce.Domain.Enums;

namespace ECommerce.Domain.Entities
{
    public class PaymentTransaction : BaseEntity
    {
        public int OrderId { get; set; }
        public Order Order { get; set; }

        public PaymentMethod PaymentMethod { get; set; }
        public string? ProviderTransactionId { get; set; }
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
