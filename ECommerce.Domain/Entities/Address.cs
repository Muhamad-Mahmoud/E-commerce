namespace ECommerce.Domain.Entities
{
    public class Address : BaseEntity
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string? Street { get; set; }
        public string? PostalCode { get; set; }
        public bool IsDefaultShipping { get; set; }
    }
}
