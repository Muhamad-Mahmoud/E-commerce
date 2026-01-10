using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Persistence
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Product Configuration
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
                entity.Property(p => p.Description).HasMaxLength(2000);
                
                entity.HasOne<Category>()
                    .WithMany(c => c.Products)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Category Configuration
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
                entity.Property(c => c.ImageUrl).HasMaxLength(500);

                entity.HasOne(c => c.ParentCategory)
                    .WithMany()
                    .HasForeignKey(c => c.ParentCategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ProductVariant Configuration
            modelBuilder.Entity<ProductVariant>(entity =>
            {
                entity.HasKey(pv => pv.Id);
                entity.Property(pv => pv.SKU).IsRequired().HasMaxLength(50);
                entity.Property(pv => pv.VariantName).IsRequired().HasMaxLength(100);
                entity.Property(pv => pv.Price).HasPrecision(18, 2);
                entity.Property(pv => pv.Color).HasMaxLength(50);
                entity.Property(pv => pv.Size).HasMaxLength(50);

                entity.HasOne(pv => pv.Product)
                    .WithMany(p => p.Variants)
                    .HasForeignKey(pv => pv.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Order Configuration
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.Id);
                entity.Property(o => o.OrderNumber).IsRequired().HasMaxLength(50);
                entity.Property(o => o.TotalAmount).HasPrecision(18, 2);
                entity.Property(o => o.UserId).IsRequired();

                entity.HasOne<ApplicationUser>()
                    .WithMany(u => u.Orders)
                    .HasForeignKey(o => o.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(o => o.OrderNumber).IsUnique();
            });

            // OrderItem Configuration
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(oi => oi.Id);
                entity.Property(oi => oi.UnitPrice).HasPrecision(18, 2);
                entity.Property(oi => oi.ItemTotal).HasPrecision(18, 2);
            });

            // PaymentTransaction Configuration
            modelBuilder.Entity<PaymentTransaction>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Amount).HasPrecision(18, 2);
            });

            // ShoppingCartItem Configuration
            modelBuilder.Entity<ShoppingCartItem>(entity =>
            {
                entity.HasKey(ci => ci.Id);
                entity.Property(ci => ci.UnitPrice).HasPrecision(18, 2);
            });

            // ApplicationUser Configuration
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(u => u.FullName).IsRequired().HasMaxLength(200);
            });

            // Address Configuration
            modelBuilder.Entity<Address>(entity =>
            {
                entity.HasKey(a => a.Id);
            });

            // Review Configuration
            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(r => r.Id);
            });

            // ShoppingCart Configuration
            modelBuilder.Entity<ShoppingCart>(entity =>
            {
                entity.HasKey(sc => sc.Id);
            });

            // ProductImage Configuration
            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.HasKey(pi => pi.Id);
            });

            // ProductVariantImage Configuration
            modelBuilder.Entity<ProductVariantImage>(entity =>
            {
                entity.HasKey(pvi => pvi.Id);
            });
        }


        public DbSet<Address> Addresses { get; set; }
        public DbSet<Category> Categories { get; set; }
        // Note: ApplicationUser is accessible via inherited 'Users' property from IdentityDbContext
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<ProductVariantImage> ProductVariantImages { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }

    }
}
