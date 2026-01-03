using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.UnitPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.ItemTotal)
                .HasPrecision(18, 2);

            modelBuilder.Entity<PaymentTransaction>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ProductVariant>()
                .Property(pv => pv.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ShoppingCartItem>()
                .Property(ci => ci.UnitPrice)
                .HasPrecision(18, 2);
        }


        public DbSet<Address> Addresses { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<PaymentTransaction>  PaymentTransactions { get; set; }
        public DbSet<Product>  Products { get; set; }
        public DbSet<ProductImage>  ProductImages { get; set; }
        public DbSet<ProductVariant>  ProductVariants { get; set; }
        public DbSet<ProductVariantImage>  ProductVariantImages { get; set; }
        public DbSet<Review>  Reviews { get; set; }
        public DbSet<ShoppingCart> shoppingCarts { get; set; }
        public DbSet<ShoppingCartItem> shoppingCartItems { get; set; }

    }
}
