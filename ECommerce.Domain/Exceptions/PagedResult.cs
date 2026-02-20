using ECommerce.Domain.Exceptions;
using System;
using System.Collections.Generic;

namespace ECommerce.Domain.Exceptions
{
    public class PagedResult<T>
    {
        public int PageNumber { get; }
        public int PageSize { get; }
        public int TotalCount { get; }
        public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;
        public IReadOnlyList<T> Items { get; }

        public PagedResult(int pageNumber, int pageSize, int totalCount, IReadOnlyList<T> items)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = totalCount;
            Items = items;
        }

        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
}
