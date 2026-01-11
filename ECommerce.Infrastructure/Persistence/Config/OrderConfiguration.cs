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

            builder.HasOne<ApplicationUser>()
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(o => o.OrderNumber).IsUnique();
        }
    }
}
