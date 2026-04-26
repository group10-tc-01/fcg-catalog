using FCG.Catalog.Application.Abstractions.Caching;
using FCG.Catalog.Domain.Models;
using FCG.Catalog.Domain.Repositories.Game;
using System.Diagnostics.CodeAnalysis;

namespace FCG.Catalog.Application.UseCases.Games.Get
{
    [ExcludeFromCodeCoverage]
    public class GetGameUseCase : IGetAllGamesUseCase
    {
        private readonly IReadOnlyGameRepository _readRepo;
        private readonly IGameCacheService _gameCacheService;

        public GetGameUseCase(IReadOnlyGameRepository readRepo, IGameCacheService? gameCacheService = null)
        {
            _readRepo = readRepo;
            _gameCacheService = gameCacheService ?? NullGameCacheService.Instance;
        }

        public async Task<PagedListResponse<GetGameOutput>> Handle(GetGameInput request,
            CancellationToken cancellationToken)
        {
            var pagination = request.Pagination ?? new PaginationParams();

            var cachedResult = await _gameCacheService.GetGameListAsync(request, pagination, cancellationToken);
            if (cachedResult is not null)
            {
                return cachedResult;
            }

            var query = _readRepo.GetAllWithFilters(
                name: request.Name,
                category: request.Category,
                minPrice: request.MinPrice,
                maxPrice: request.MaxPrice);

            var totalCount = query.Count();

            var pagedQuery = query
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize);

            var games = pagedQuery.ToList();

            var items = games.Where(x => x is not null).Select(x =>
            {
                var game = x!;
                var originalPrice = game.Price.Value;

                var activePromotion = game.GetActivePromotion();
                var finalPrice = game.CalculateDiscountedPrice(activePromotion);

                ActivePromotionDto? promotionDto = null;

                if (activePromotion != null)
                {
                    promotionDto = new ActivePromotionDto
                    {
                        PromotionId = activePromotion.Id,
                        DiscountPercentage = activePromotion.DiscountPercentage.Value,
                        StartDate = activePromotion.StartDate,
                        EndDate = activePromotion.EndDate
                    };
                }

                return new GetGameOutput()
                {
                    Id = game.Id,
                    Category = game.Category.ToString(),
                    Description = game.Description,
                    Title = game.Title.Value,
                    Price = originalPrice,
                    FinalPrice = finalPrice,
                    ActivePromotion = promotionDto
                };
            }).ToList();

            var result = new PagedListResponse<GetGameOutput>(items, totalCount, pagination.PageNumber, pagination.PageSize);
            await _gameCacheService.SetGameListAsync(request, pagination, result, cancellationToken);

            return result;
        }
    }
}
