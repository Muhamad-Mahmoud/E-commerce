using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Config
{
    /// <summary>
    /// Entity configuration for ProductVariantImage.
    /// </summary>
    public class ProductVariantImageConfiguration : IEntityTypeConfiguration<ProductVariantImage>
    {
        public void Configure(EntityTypeBuilder<ProductVariantImage> builder)
        {
            builder.HasKey(pvi => pvi.Id);
        }
    }
}
