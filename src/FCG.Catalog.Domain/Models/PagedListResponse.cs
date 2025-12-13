using System;
using System.Collections.Generic;

namespace FCG.Catalog.Domain.Models
{
    public sealed class PagedListResponse<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;

        public PagedListResponse(List<T> items, int totalCount, int currentPage, int pageSize)
        {
            Items.AddRange(items);
            TotalCount = totalCount;
            PageSize = pageSize;
            CurrentPage = currentPage;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        }
    }
}
