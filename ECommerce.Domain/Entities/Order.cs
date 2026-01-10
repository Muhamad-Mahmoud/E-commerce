using ECommerce.Domain.Enums;

namespace ECommerce.Domain.Entities
{
    public class Order
    {
        // Private parameterless constructor for EF Core
        private Order() { }

        // Public constructor with validation
        public Order(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID cannot be empty", nameof(userId));

            UserId = userId;
            OrderNumber = GenerateOrderNumber();
            CreatedAt = DateTime.UtcNow;
            Status = OrderStatus.Pending;
            PaymentStatus = PaymentStatus.Pending;
            TotalAmount = 0;
        }

        public int Id { get; private set; }

        public string UserId { get; private set; }
        public string OrderNumber { get; private set; }

        public decimal TotalAmount { get; private set; }

        public OrderStatus Status { get; private set; } = OrderStatus.Pending;
        public PaymentStatus PaymentStatus { get; private set; } = PaymentStatus.Pending;

        public DateTime CreatedAt { get; private set; }

        // Business logic methods
        public void UpdateStatus(OrderStatus newStatus)
        {
            if (Status == OrderStatus.Cancelled)
                throw new InvalidOperationException("Cannot update a cancelled order");

            Status = newStatus;
        }

        public void UpdatePaymentStatus(PaymentStatus newPaymentStatus)
        {
            PaymentStatus = newPaymentStatus;
        }

        public void Cancel()
        {
            if (Status == OrderStatus.Delivered)
                throw new InvalidOperationException("Cannot cancel a delivered order");

            Status = OrderStatus.Cancelled;
        }

        public void UpdateTotal(decimal totalAmount)
        {
            if (totalAmount < 0)
                throw new ArgumentException("Total amount cannot be negative", nameof(totalAmount));

            TotalAmount = totalAmount;
        }

        private static string GenerateOrderNumber()
        {
            return $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper()}";
        }
    }
}
