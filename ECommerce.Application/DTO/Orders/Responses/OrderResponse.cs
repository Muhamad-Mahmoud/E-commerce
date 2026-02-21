namespace ECommerce.Application.DTO.Orders.Responses
{
    public class OrderResponse
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public ShippingAddressResponse ShippingAddress { get; set; } = null!;
        public List<OrderItemResponse> OrderItems { get; set; } = new();
    }
}
