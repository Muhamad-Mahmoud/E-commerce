namespace ECommerce.Application.DTO.Pagination
{
    public class ProductParams : PaginationParams
    {
        public const string SortPriceAsc = "priceAsc";

        public const string SortPriceDesc = "priceDesc";

        public const string SortNewest = "newest";

        public string? Search { get; set; }

        public int? CategoryId { get; set; }

        public decimal? MinPrice { get; set; }

        public decimal? MaxPrice { get; set; }

        public string Sort { get; set; } = SortNewest;
    }
}
