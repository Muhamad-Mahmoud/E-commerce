namespace ECommerce.Application.DTO.Wishlist
{
    public class WishlistItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class WishlistDto
    {
        public int Id { get; set; }
        public List<WishlistItemDto> Items { get; set; } = new();
    }
}
