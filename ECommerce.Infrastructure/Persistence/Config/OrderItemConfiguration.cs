using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Config
{
    /// <summary>
    /// Entity configuration for OrderItem.
    /// </summary>
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.HasKey(oi => oi.Id);
            builder.Property(oi => oi.UnitPrice).HasPrecision(18, 2);
            builder.Property(oi => oi.ItemTotal).HasPrecision(18, 2);
        }
    }
}
