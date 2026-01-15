namespace ECommerce.Application.DTO.Orders.Responses
{
    public class OrderResponse
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public string UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } // Return as string for frontend friendly
        public string PaymentStatus { get; set; } // Return as string
        public DateTime CreatedAt { get; set; }
        public List<OrderItemResponse> OrderItems { get; set; }
    }
}
