namespace ECommerce.Application.DTO.Products
{
    public class ProductParams
    {
        public string? Search { get; set; }
        public int? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? Sort { get; set; } // "priceAsc", "priceDesc", "newest"
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
