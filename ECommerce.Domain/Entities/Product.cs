using ECommerce.Domain.Enums;

namespace ECommerce.Domain.Entities
{
    public class Product
    {
        // Private parameterless constructor for EF Core
        private Product() { }

        // Public constructor with validation
        public Product(string name, string? description, int categoryId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Product name cannot be empty", nameof(name));

            if (categoryId <= 0)
                throw new ArgumentException("Invalid category ID", nameof(categoryId));

            Name = name;
            Description = description;
            CategoryId = categoryId;
            Status = ProductStatus.Draft;
            Images = new List<ProductImage>();
            Variants = new List<ProductVariant>();
            Reviews = new List<Review>();
        }

        public int Id { get; private set; }

        public string Name { get; private set; }

        public string? Description { get; private set; }

        public int CategoryId { get; private set; }

        public ProductStatus Status { get; private set; }

        public ICollection<ProductImage> Images { get; private set; } = new List<ProductImage>();
        public ICollection<ProductVariant> Variants { get; private set; } = new List<ProductVariant>();
        public ICollection<Review> Reviews { get; private set; } = new List<Review>();

        // Business logic methods
        public void UpdateInfo(string name, string? description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Product name cannot be empty", nameof(name));

            Name = name;
            Description = description;
        }

        public void Publish()
        {
            if (Status == ProductStatus.Published)
                throw new InvalidOperationException("Product is already published");

            if (!Variants.Any())
                throw new InvalidOperationException("Cannot publish product without variants");

            Status = ProductStatus.Published;
        }

        public void Archive()
        {
            Status = ProductStatus.Archived;
        }

        public void ChangeCategory(int categoryId)
        {
            if (categoryId <= 0)
                throw new ArgumentException("Invalid category ID", nameof(categoryId));

            CategoryId = categoryId;
        }
    }
}
