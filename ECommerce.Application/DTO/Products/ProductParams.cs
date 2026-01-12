namespace ECommerce.Application.DTO.Products
{
    public class ProductParams
    {
        public const string SortPriceAsc = "priceAsc";
        public const string SortPriceDesc = "priceDesc";
        public const string SortNewest = "newest";

        public string? Search { get; set; }
        public int? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? Sort { get; set; } = SortNewest; // Default sort
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
