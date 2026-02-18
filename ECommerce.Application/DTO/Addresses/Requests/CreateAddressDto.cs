using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTO.Addresses.Requests
{
    public class CreateAddressDto
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public string Country { get; set; } = string.Empty;

        [Required]
        public string City { get; set; } = string.Empty;

        public string? Street { get; set; }
        public string? PostalCode { get; set; }
        public bool IsDefaultShipping { get; set; }
    }
}
