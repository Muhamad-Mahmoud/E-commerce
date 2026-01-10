using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Entities
{
    public class Category
    {
        // Private parameterless constructor for EF Core
        private Category() { }

        // Public constructor with validation
        public Category(string name, string? imageUrl = null, int? parentCategoryId = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Category name cannot be empty", nameof(name));

            Name = name;
            ImageUrl = imageUrl;
            ParentCategoryId = parentCategoryId;
            Products = new List<Product>();
        }

        public int Id { get; private set; }
        public string Name { get; private set; }
        public string? ImageUrl { get; private set; }

        public int? ParentCategoryId { get; private set; }
        public Category? ParentCategory { get; private set; }

        public ICollection<Product> Products { get; private set; }

        // Business logic methods
        public void UpdateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Category name cannot be empty", nameof(name));

            Name = name;
        }

        public void UpdateImage(string? imageUrl)
        {
            ImageUrl = imageUrl;
        }
    }
}
