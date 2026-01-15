using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using ECommerce.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Persistence
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, AppDbContext context)
        {
            // Seed Roles
            await SeedRolesAsync(roleManager);

            // Seed Users
            await SeedUsersAsync(userManager);

            // Seed Catalog (Categories & Products)
            await SeedCatalogAsync(context);
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));

            if (!await roleManager.RoleExistsAsync("User"))
                await roleManager.CreateAsync(new IdentityRole("User"));
        }

        private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
        {
            var adminData = new UserSeedDto("admin", "admin@ecommerce.com", "Admin", "Admin", true);
            await EnsureUserAsync(userManager, adminData);

            var userData = new UserSeedDto("user", "user@ecommerce.com", "Test User", "User", false);
            await EnsureUserAsync(userManager, userData);
        }

        private static async Task EnsureUserAsync(UserManager<ApplicationUser> userManager, UserSeedDto data)
        {
            var user = await userManager.FindByEmailAsync(data.Email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = data.Username,
                    Email = data.Email,
                    FullName = data.FullName,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(user, "Pa$$w0rd123");
                await userManager.AddToRoleAsync(user, data.Role);
            }
            else if (data.ResetPassword)
            {
                // Ensure password matches dev environment expectations
                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                await userManager.ResetPasswordAsync(user, token, "Pa$$w0rd123");
            }
        }

        private static async Task SeedCatalogAsync(AppDbContext context)
        {
            if (await context.Categories.AnyAsync()) return;

            // Categories
            var electronics = new Category { Name = "Electronics", ImageUrl = "https://placehold.co/600x400?text=Electronics" };
            var fashion = new Category { Name = "Fashion", ImageUrl = "https://placehold.co/600x400?text=Fashion" };

            await context.Categories.AddRangeAsync(electronics, fashion);
            await context.SaveChangesAsync();

            var laptops = new Category { Name = "Laptops", ParentCategoryId = electronics.Id, ImageUrl = "https://placehold.co/600x400?text=Laptops" };
            var smartphones = new Category { Name = "Smartphones", ParentCategoryId = electronics.Id, ImageUrl = "https://placehold.co/600x400?text=Smartphones" };

            await context.Categories.AddRangeAsync(laptops, smartphones);
            await context.SaveChangesAsync();

            // Products
            var products = new List<Product>
            {
                new Product
                {
                    Name = "MacBook Pro 16",
                    Description = "Apple M3 Max chip, 32GB RAM, 1TB SSD",
                    CategoryId = laptops.Id,
                    Status = ProductStatus.Published,
                    Variants = new List<ProductVariant>
                    {
                        new ProductVariant { SKU = "MAC-16-SILVER", VariantName = "Silver Base", Price = 2499, StockQuantity = 10, Color = "Silver" },
                        new ProductVariant { SKU = "MAC-16-SPACE", VariantName = "Space Black", Price = 2599, StockQuantity = 5, Color = "Space Black" }
                    },
                    Images = new List<ProductImage>
                    {
                        new ProductImage { ImageUrl = "https://placehold.co/600x400?text=MacBook+Front", IsPrimary = true }
                    }
                },
                new Product
                {
                    Name = "iPhone 15 Pro",
                    Description = "Titanium design, A17 Pro chip",
                    CategoryId = smartphones.Id,
                    Status = ProductStatus.Published,
                    Variants = new List<ProductVariant>
                    {
                        new ProductVariant { SKU = "IPH-15-TI", VariantName = "Natural Titanium", Price = 999, StockQuantity = 20 }
                    },
                    Images = new List<ProductImage>
                    {
                        new ProductImage { ImageUrl = "https://placehold.co/600x400?text=iPhone+15", IsPrimary = true }
                    }
                }
            };

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
        }
    }
}
