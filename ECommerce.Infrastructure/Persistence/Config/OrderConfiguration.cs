using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Config
{
    /// <summary>
    /// Entity configuration for Order.
    /// </summary>
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);
            builder.Property(o => o.OrderNumber).IsRequired().HasMaxLength(50);
            builder.Property(o => o.TotalAmount).HasPrecision(18, 2);
            builder.Property(o => o.UserId).IsRequired();
            builder.Property(o => o.RowVersion).IsRowVersion();

            builder.HasOne<ApplicationUser>()
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.OwnsOne(o => o.ShippingAddress, sa =>
            {
                sa.Property(p => p.FullName).IsRequired().HasMaxLength(100);
                sa.Property(p => p.Phone).IsRequired().HasMaxLength(20);
                sa.Property(p => p.Country).IsRequired().HasMaxLength(50);
                sa.Property(p => p.City).IsRequired().HasMaxLength(50);
                sa.Property(p => p.Street).HasMaxLength(200);
                sa.Property(p => p.PostalCode).HasMaxLength(20);
            });

            builder.HasIndex(o => o.OrderNumber).IsUnique();
        }
    }
}
