using ECommerce.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        // Parameterless constructor for Identity
        public ApplicationUser()
        {
            Addresses = new List<Address>();
            ShoppingCarts = new List<ShoppingCart>();
            Orders = new List<Order>();
            Reviews = new List<Review>();
        }

        // Constructor with validation
        public ApplicationUser(string fullName, string email, string phoneNumber) : this()
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Full name cannot be empty", nameof(fullName));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty", nameof(email));

            FullName = fullName;
            Email = email;
            UserName = email;
            PhoneNumber = phoneNumber;
        }

        public string FullName { get; set; }
        public new string PhoneNumber { get; set; }

        public ICollection<Address> Addresses { get; set; }
        public ICollection<ShoppingCart> ShoppingCarts { get; set; }
        public ICollection<Order> Orders { get; set; }
        public ICollection<Review> Reviews { get; set; }
    }
}
