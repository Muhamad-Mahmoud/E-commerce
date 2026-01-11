using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Config
{
    /// <summary>
    /// Entity configuration for ShoppingCartItem.
    /// </summary>
    public class ShoppingCartItemConfiguration : IEntityTypeConfiguration<ShoppingCartItem>
    {
        public void Configure(EntityTypeBuilder<ShoppingCartItem> builder)
        {
            builder.HasKey(ci => ci.Id);
            builder.Property(ci => ci.UnitPrice).HasPrecision(18, 2);
        }
    }
}
