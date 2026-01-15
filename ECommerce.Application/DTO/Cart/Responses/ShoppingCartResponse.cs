
namespace ECommerce.Application.DTO.Cart.Responses
{
    public class ShoppingCartResponse
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public List<ShoppingCartItemResponse> Items { get; set; } = new List<ShoppingCartItemResponse>();
    }
}
