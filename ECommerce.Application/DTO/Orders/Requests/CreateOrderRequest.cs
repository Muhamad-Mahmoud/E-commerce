namespace ECommerce.Application.DTO.Orders.Requests
{
    /// <summary>
    /// Request DTO for creating a new order.
    /// </summary>
    public class CreateOrderRequest
    {
        /// <summary>
        /// The ID of the shipping address to use for this order.
        /// </summary>
        public int ShippingAddressId { get; set; }
    }
}
