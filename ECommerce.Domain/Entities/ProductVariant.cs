using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Entities
{
    public class ProductVariant
    {
        // Private parameterless constructor for EF Core
        private ProductVariant() { }

        // Public constructor with validation
        public ProductVariant(int productId, string sku, string variantName, decimal price, int stockQuantity)
        {
            if (productId <= 0)
                throw new ArgumentException("Invalid product ID", nameof(productId));

            if (string.IsNullOrWhiteSpace(sku))
                throw new ArgumentException("SKU cannot be empty", nameof(sku));

            if (string.IsNullOrWhiteSpace(variantName))
                throw new ArgumentException("Variant name cannot be empty", nameof(variantName));

            if (price < 0)
                throw new ArgumentException("Price cannot be negative", nameof(price));

            if (stockQuantity < 0)
                throw new ArgumentException("Stock quantity cannot be negative", nameof(stockQuantity));

            ProductId = productId;
            SKU = sku;
            VariantName = variantName;
            Price = price;
            StockQuantity = stockQuantity;
            IsActive = true;
            Images = new List<ProductVariantImage>();
            CartItems = new List<ShoppingCartItem>();
            OrderItems = new List<OrderItem>();
        }

        public int Id { get; private set; }
        public int ProductId { get; private set; }
        public Product Product { get; private set; }

        public string SKU { get; private set; }
        public string VariantName { get; private set; }
        public decimal Price { get; private set; }

        public int StockQuantity { get; private set; }
        public string? Color { get; private set; }
        public string? Size { get; private set; }

        public bool IsActive { get; private set; } = true;

        public ICollection<ProductVariantImage> Images { get; private set; }
        public ICollection<ShoppingCartItem> CartItems { get; private set; }
        public ICollection<OrderItem> OrderItems { get; private set; }

        // Business logic methods
        public void UpdatePrice(decimal newPrice)
        {
            if (newPrice < 0)
                throw new ArgumentException("Price cannot be negative", nameof(newPrice));

            Price = newPrice;
        }

        public void UpdateStock(int quantity)
        {
            if (quantity < 0)
                throw new ArgumentException("Stock quantity cannot be negative", nameof(quantity));

            StockQuantity = quantity;
        }

        public void ReduceStock(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive", nameof(quantity));

            if (StockQuantity < quantity)
                throw new InvalidOperationException("Insufficient stock");

            StockQuantity -= quantity;
        }

        public void IncreaseStock(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive", nameof(quantity));

            StockQuantity += quantity;
        }

        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void SetAttributes(string? color, string? size)
        {
            Color = color;
            Size = size;
        }
    }
}
