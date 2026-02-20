namespace ECommerce.Domain.Exceptions;

public static class DomainErrors
{
    public static class General
    {
        public static readonly Error IdMismatch = new("General.IdMismatch", "The ID in the URL does not match the ID in the request body.", 400);
        public static readonly Error UnAuthorized = Error.Unauthorized("You are not authorized to perform this action.");
    }

    public static class Order
    {
        public static readonly Error NotFound = new("Order.NotFound", "The order was not found.", 404);
        public static readonly Error Unauthorized = Error.Unauthorized("You are not authorized to access this order.");
        public static readonly Error EmptyCart = new("Order.EmptyCart", "The shopping cart is empty.", 400);
        public static readonly Error InsufficientStock = new("Order.InsufficientStock", "One or more items in the cart have insufficient stock.", 400);
        public static readonly Error InvalidStatusTransition = new("Order.InvalidStatusTransition", "The order status transition is invalid.", 400);
        public static readonly Error ConcurrencyConflict = new("Order.ConcurrencyConflict", "A concurrency conflict occurred while processing the order. Please try again.", 409);
        public static readonly Error ShippingAddressRequired = new("Order.ShippingAddressRequired", "A shipping address is required to create an order.", 400);
    }

    public static class User
    {
        public static readonly Error IdRequired = new("User.IdRequired", "The user ID is required.", 400);
    }

    public static class Product
    {
        public static readonly Error NotFound = new("Product.NotFound", "The product was not found.", 404);
        public static readonly Error VariantNotFound = new("Product.VariantNotFound", "The product variant was not found.", 404);
        public static readonly Error IdMismatch = new("Product.IdMismatch", "The product ID in the URL does not match the ID in the request body.", 400);
    }

    public static class Category
    {
        public static readonly Error NotFound = new("Category.NotFound", "The category was not found.", 404);
        public static readonly Error InvalidParent = new("Category.InvalidParent", "The specified parent category does not exist.", 400);
    }

    public static class Cart
    {
        public static readonly Error ItemNotFound = new("Cart.ItemNotFound", "The cart item was not found.", 404);
        public static readonly Error VariantNotFound = new("Cart.VariantNotFound", "The product variant was not found.", 404);
        public static readonly Error InsufficientStock = new("Cart.InsufficientStock", "The requested quantity exceeds the available stock.", 400);
    }

    public static class Wishlist
    {
        public static readonly Error ItemNotFound = new("Wishlist.ItemNotFound", "The item was not found in the wishlist.", 404);
    }

    public static class Address
    {
        public static readonly Error NotFound = new("Address.NotFound", "The address was not found.", 404);
    }

    public static class Review
    {
        public static readonly Error ProductNotFound = new("Review.ProductNotFound", "The product was not found.", 404);
        public static readonly Error NotFound = new("Review.NotFound", "The review was not found.", 404);
        public static readonly Error Unauthorized = Error.Unauthorized("You are not authorized to delete this review.");
        public static readonly Error DuplicateReview = new("Review.DuplicateReview", "You have already submitted a review for this product.", 400);
    }

    public static class Authentication
    {
        public static readonly Error InvalidCredentials = new("Auth.InvalidCredentials", "Invalid email or password.", 401);
        public static readonly Error RegistrationError = new("Auth.RegistrationError", "An error occurred during registration.", 400);
        public static readonly Error InvalidRefreshToken = new("Auth.InvalidRefreshToken", "Invalid or expired refresh token.", 401);
        public static readonly Error UserNotFound = new("Auth.UserNotFound", "User not found.", 404);
    }
}
