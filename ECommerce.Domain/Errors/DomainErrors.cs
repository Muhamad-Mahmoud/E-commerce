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
        public static readonly Error IdMismatch = new("Product.IdMismatch", "The product ID in the URL does not match the ID in the request body.");
    }

    public static class Category
    {
        public static readonly Error NotFound = new("Category.NotFound", "The category was not found.");
        public static readonly Error InvalidParent = new("Category.InvalidParent", "The specified parent category does not exist.");
    }

    public static class Cart
    {
        public static readonly Error ItemNotFound = new("Cart.ItemNotFound", "The cart item was not found.");
        public static readonly Error VariantNotFound = new("Cart.VariantNotFound", "The product variant was not found.");
        public static readonly Error InsufficientStock = new("Cart.InsufficientStock", "The requested quantity exceeds the available stock.");
    }

    public static class Wishlist
    {
        public static readonly Error ItemNotFound = new("Wishlist.ItemNotFound", "The item was not found in the wishlist.");
    }

    public static class Address
    {
        public static readonly Error NotFound = new("Address.NotFound", "The address was not found.");
    }

    public static class Review
    {
        public static readonly Error ProductNotFound = new("Review.ProductNotFound", "The product was not found.");
        public static readonly Error NotFound = new("Review.NotFound", "The review was not found.");
        public static readonly Error Unauthorized = new("Review.Unauthorized", "You are not authorized to delete this review.");
        public static readonly Error DuplicateReview = new("Review.DuplicateReview", "You have already submitted a review for this product.");
    }
}
