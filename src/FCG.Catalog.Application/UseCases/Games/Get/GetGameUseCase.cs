using FCG.Catalog.Domain.Models;
using FCG.Catalog.Domain.Repositories.Game;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FCG.Catalog.Application.UseCases.Games.Get
{
    [ExcludeFromCodeCoverage]
    public class GetGameUseCase : IGetAllGamesUseCase
    {
        private readonly IReadOnlyGameRepository _readRepo;

        public GetGameUseCase(IReadOnlyGameRepository readRepo)
        {
            _readRepo = readRepo;
        }

        public Task<PagedListResponse<GetGameOutput>> Handle(GetGameInput request,
            CancellationToken cancellationToken)
        {
            var pagination = request.Pagination ?? new PaginationParams();

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

            var items = games.Select(x =>
            {
                var originalPrice = x.Price.Value;

                var activePromotion = x.GetActivePromotion();
                var finalPrice = x.CalculateDiscountedPrice(activePromotion);

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
                    Id = x.Id,
                    Category = x.Category.ToString(),
                    Description = x.Description,
                    Title = x.Title.Value,
                    Price = originalPrice,
                    FinalPrice = finalPrice,
                    ActivePromotion = promotionDto
                };
            }).ToList();

            var result = new PagedListResponse<GetGameOutput>(items, totalCount, pagination.PageNumber, pagination.PageSize);
            return Task.FromResult(result);
        }
    }
}
