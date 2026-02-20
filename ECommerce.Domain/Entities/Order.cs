using ECommerce.Domain.Enums;

namespace ECommerce.Domain.Entities
{
    /// <summary>
    /// Represents a customer order in the system.
    /// Implements encapsulation while allowing infrastructure and tests access via internal setters.
    /// </summary>
    public class Order : BaseEntity
    {
        public string UserId { get; internal set; } = string.Empty;
        public string OrderNumber { get; internal set; } = string.Empty;
        public decimal TotalAmount { get; internal set; }
        public OrderStatus Status { get; internal set; }
        public PaymentStatus PaymentStatus { get; internal set; }
        public DateTime CreatedAt { get; internal set; }

        public ShippingAddress ShippingAddress { get; internal set; } = null!;

        private readonly List<OrderItem> _orderItems = new();
        public virtual ICollection<OrderItem> OrderItems => _orderItems;

        private readonly List<PaymentTransaction> _paymentTransactions = new();
        public virtual ICollection<PaymentTransaction> PaymentTransactions => _paymentTransactions;

        // EF Core constructor
        internal Order() { }

        /// <summary>
        /// Creates a new Order instance.
        /// </summary>
        public Order(string userId, string orderNumber, ShippingAddress shippingAddress)
        {
            UserId = userId;
            OrderNumber = orderNumber;
            ShippingAddress = shippingAddress;
            Status = OrderStatus.Pending;
            PaymentStatus = PaymentStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Adds an item to the order and updates the total amount.
        /// </summary>
        public void AddItem(ProductVariant variant, int quantity)
        {
            var orderItem = new OrderItem
            {
                ProductVariantId = variant.Id,
                ProductName = $"{variant.Product?.Name} - {variant.SKU}",
                UnitPrice = variant.Price,
                Quantity = quantity,
                ItemTotal = variant.Price * quantity
            };

            _orderItems.Add(orderItem);
            TotalAmount += orderItem.ItemTotal;
        }

        /// <summary>
        /// Updates the order status ensuring valid transitions.
        /// </summary>
        public void UpdateStatus(OrderStatus newStatus)
        {
            if (!IsValidTransition(Status, newStatus))
                throw new InvalidOperationException($"Invalid status transition from {Status} to {newStatus}");

            Status = newStatus;
        }

        /// <summary>
        /// Cancels the order if it's in a cancellable state.
        /// </summary>
        public void Cancel()
        {
            UpdateStatus(OrderStatus.Cancelled);
        }

        /// <summary>
        /// Updates the payment status of the order.
        /// </summary>
        public void UpdatePaymentStatus(PaymentStatus newStatus)
        {
            PaymentStatus = newStatus;
        }

        private static bool IsValidTransition(OrderStatus current, OrderStatus next)
        {
            return current switch
            {
                OrderStatus.Pending => next is OrderStatus.Processing or OrderStatus.Cancelled,
                OrderStatus.Processing => next is OrderStatus.Shipped or OrderStatus.Cancelled,
                OrderStatus.Shipped => next is OrderStatus.Delivered,
                OrderStatus.Delivered => false,
                OrderStatus.Cancelled => false,
                _ => false
            };
        }
    }
}
