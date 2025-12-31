using ECommerce.Core.Domain.Entities;
using ECommerce.Core.Domain.Enums;

public class Product
{
    public int Id { get; set; }

    public string Name { get; private set; }

    public string? Description { get; private set; }

    public int CategoryId { get; set; }

    public ProductStatus Status { get; set; }

    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
