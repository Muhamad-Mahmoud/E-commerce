using ECommerce.Domain.Enums;

namespace ECommerce.Domain.Entities
{
    public class Order : BaseEntity
    {
        public string UserId { get; set; }
        public string OrderNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
        public ICollection<PaymentTransaction> PaymentTransactions { get; set; }
    }
}
