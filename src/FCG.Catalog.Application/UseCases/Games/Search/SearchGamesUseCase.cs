using FCG.Catalog.Domain.Models;
using FCG.Catalog.Domain.Repositories.Game;
using System.Diagnostics.CodeAnalysis;

namespace FCG.Catalog.Application.UseCases.Games.Search
{
    [ExcludeFromCodeCoverage]
    public class SearchGamesUseCase : ISearchGamesUseCase
    {
        private readonly IGameSearchRepository _gameSearchRepository;

        public SearchGamesUseCase(IGameSearchRepository gameSearchRepository)
        {
            _gameSearchRepository = gameSearchRepository;
        }

        public async Task<PagedListResponse<SearchGameOutput>> Handle(
            SearchGamesInput request,
            CancellationToken cancellationToken)
        {
            var result = await _gameSearchRepository.SearchAsync(
                request.Term.Trim(),
                request.ToPaginationParams(),
                cancellationToken);

            var items = result.Items
                .Select(game => new SearchGameOutput
                {
                    Id = game.Id,
                    Title = game.Title,
                    Description = game.Description,
                    Price = game.Price,
                    Category = game.Category,
                    DiscountedPrice = game.DiscountedPrice,
                    IsActive = game.IsActive,
                    IndexedAt = game.IndexedAt,
                    Score = game.Score
                })
                .ToList();

            return new PagedListResponse<SearchGameOutput>(
                items,
                result.TotalCount,
                result.CurrentPage,
                result.PageSize);
        }
    }
}
