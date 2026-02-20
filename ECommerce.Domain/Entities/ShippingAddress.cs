namespace ECommerce.Domain.Entities
{
    /// <summary>
    /// Represents a snapshot of the shipping address at the time an order was placed.
    /// This ensures that if the user changes their address later, the order's history remains accurate.
    /// </summary>
    public class ShippingAddress
    {
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string? Street { get; set; }
        public string? PostalCode { get; set; }
    }
}
