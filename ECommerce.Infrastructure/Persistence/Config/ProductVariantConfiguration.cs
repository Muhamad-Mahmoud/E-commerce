using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Config
{
    /// <summary>
    /// Entity configuration for ProductVariant.
    /// </summary>
    public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
    {
        public void Configure(EntityTypeBuilder<ProductVariant> builder)
        {
            builder.HasKey(pv => pv.Id);
            builder.Property(pv => pv.SKU).IsRequired().HasMaxLength(50);
            builder.Property(pv => pv.VariantName).IsRequired().HasMaxLength(100);
            builder.Property(pv => pv.Price).HasPrecision(18, 2);
            builder.Property(pv => pv.Color).HasMaxLength(50);
            builder.Property(pv => pv.Size).HasMaxLength(50);

            builder.HasOne(pv => pv.Product)
                .WithMany(p => p.Variants)
                .HasForeignKey(pv => pv.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
