using FCG.Catalog.Application.Abstractions.Messaging;
using FCG.Catalog.Domain.Models;

namespace FCG.Catalog.Application.UseCases.Games.Search
{
    public interface ISearchGamesUseCase : IQueryHandler<SearchGamesInput, PagedListResponse<SearchGameOutput>>
    {
    }
}
