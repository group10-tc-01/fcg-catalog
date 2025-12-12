using MediatR;
using FCG.Catalog.Domain.Enum;
using FCG.Catalog.Domain.Models;

namespace FCG.Catalog.Application.UseCases.Games.Get
{
    public class GetGameInput : IRequest<PagedListResponse<GetGameOutput>>
    {
        public string? Name { get; init; }
        public GameCategory? Category { get; init; }
        public decimal? MinPrice { get; init; }
        public decimal? MaxPrice { get; init; }
        public PaginationParams? Pagination { get; init; }

        public GetGameInput() { }

        public GetGameInput(Guid id)
        {
            // compatibility constructor for single id lookups - map to Name (legacy)
            Name = id.ToString();
            Pagination = new PaginationParams { PageNumber = 1, PageSize = 1 };
        }
    }
}
