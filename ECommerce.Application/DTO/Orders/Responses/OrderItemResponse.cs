
namespace ECommerce.Application.DTO.Orders.Responses
{
    public class OrderItemResponse
    {
        public int ProductVariantId { get; set; }
        public string ProductName { get; set; }
        public string? ProductSku { get; set; } // Sometimes useful
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal ItemTotal { get; set; }
    }
}
