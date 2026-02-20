using ECommerce.Domain.Enums;

namespace ECommerce.Domain.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public ProductStatus Status { get; set; }

        public ICollection<ProductImage> Images { get; set; }
        public ICollection<ProductVariant> Variants { get; set; }
        public ICollection<Review> Reviews { get; set; }
    }
}
