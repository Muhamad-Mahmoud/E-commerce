using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Config
{
    /// <summary>
    /// Entity configuration for RefreshToken.
    /// </summary>
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasKey(rt => rt.Id);
            
            builder.Property(rt => rt.Token)
                .IsRequired()
                .HasMaxLength(500);
            
            builder.Property(rt => rt.UserId)
                .IsRequired();
            
            builder.Property(rt => rt.ExpiresAt)
                .IsRequired();
            
            builder.Property(rt => rt.CreatedAt)
                .IsRequired();
            
            // Relationship with ApplicationUser
            builder.HasOne<ECommerce.Infrastructure.Identity.ApplicationUser>()
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Index for faster lookup
            builder.HasIndex(rt => rt.Token)
                .IsUnique();
            
            builder.HasIndex(rt => rt.UserId);
        }
    }
}
