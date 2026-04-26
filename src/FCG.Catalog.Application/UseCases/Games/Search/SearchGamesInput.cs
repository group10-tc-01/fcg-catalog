using FCG.Catalog.Application.Abstractions.Messaging;
using FCG.Catalog.Domain.Models;
using System.Diagnostics.CodeAnalysis;

namespace FCG.Catalog.Application.UseCases.Games.Search
{
    [ExcludeFromCodeCoverage]
    public class SearchGamesInput : IQuery<PagedListResponse<SearchGameOutput>>
    {
        public string Term { get; init; } = string.Empty;

        public int PageNumber { get; init; } = 1;

        public int PageSize { get; init; } = 10;

        public PaginationParams ToPaginationParams()
        {
            return new PaginationParams
            {
                PageNumber = PageNumber,
                PageSize = PageSize
            };
        }
    }
}
