namespace ECommerce.Application.DTO.Addresses.Responses
{
    public class AddressDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string? Street { get; set; }
        public string? PostalCode { get; set; }
        public bool IsDefaultShipping { get; set; }
    }
}
