using ECommerce.Domain.Enums;

namespace ECommerce.Application.DTO.Pagination
{
    public class OrderParams : PaginationParams
    {
        public const string SortDateAsc = "date_asc";

        public const string SortDateDesc = "date_desc";

        public const string SortAmountAsc = "amount_asc";

        public const string SortAmountDesc = "amount_desc";

        private string _sort = SortDateDesc;

        public OrderStatus? Status { get; set; }

        public PaymentStatus? PaymentStatus { get; set; }

        public string? Search { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public decimal? MinAmount { get; set; }

        public decimal? MaxAmount { get; set; }
        public string Sort
        {
            get => _sort;
            set => _sort = IsValidSort(value) ? value : SortDateDesc;
        }
        private static bool IsValidSort(string? sortValue)
        {
            return sortValue switch
            {
                SortDateAsc or SortDateDesc or SortAmountAsc or SortAmountDesc => true,
                _ => false
            };
        }
    }
}
