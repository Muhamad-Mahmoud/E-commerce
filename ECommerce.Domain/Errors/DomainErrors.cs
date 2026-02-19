using ECommerce.Domain.Shared;

namespace ECommerce.Domain.Errors;

public static class DomainErrors
{
    public static class Order
    {
        public static readonly Error NotFound = new("Order.NotFound", "The order was not found.");
        public static readonly Error Unauthorized = new("Order.Unauthorized", "You are not authorized to access this order.");
        public static readonly Error EmptyCart = new("Order.EmptyCart", "The shopping cart is empty.");
        public static readonly Error InsufficientStock = new("Order.InsufficientStock", "One or more items in the cart have insufficient stock.");
        public static readonly Error InvalidStatusTransition = new("Order.InvalidStatusTransition", "The order status transition is invalid.");
        public static readonly Error ConcurrencyConflict = new("Order.ConcurrencyConflict", "A concurrency conflict occurred while processing the order. Please try again.");
    }

    public static class User
    {
        public static readonly Error IdRequired = new("User.IdRequired", "The user ID is required.");
    }

    public static class Product
    {
        public static readonly Error NotFound = new("Product.NotFound", "The product was not found.");
        public static readonly Error VariantNotFound = new("Product.VariantNotFound", "The product variant was not found.");
    }
}
