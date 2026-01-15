namespace ECommerce.Application.DTO.Pagination
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();

        public int TotalCount { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 1;

        public bool HasNextPage => PageNumber < TotalPages;

        public bool HasPreviousPage => PageNumber > 1;
    }
}
