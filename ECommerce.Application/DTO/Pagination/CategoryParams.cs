namespace ECommerce.Application.DTO.Pagination
{
    public class CategoryParams
    {
        public string? Search { get; set; }
        public int? ParentCategoryId { get; set; }
        public bool? IncludeSubCategories { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
