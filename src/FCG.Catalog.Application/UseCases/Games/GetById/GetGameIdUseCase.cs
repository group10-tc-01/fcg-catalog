using FCG.Catalog.Application.Abstractions.Caching;
using FCG.Catalog.Domain.Exception;
using FCG.Catalog.Domain.Repositories.Game;
using FCG.Catalog.Messages;
using System.Diagnostics.CodeAnalysis;

namespace FCG.Catalog.Application.UseCases.Games.GetById
{
    [ExcludeFromCodeCoverage]
    public class GetGameIdUseCase : IGetGameIdUseCase
    {
        private readonly IReadOnlyGameRepository _gameRepository;
        private readonly IGameCacheService _gameCacheService;

        public GetGameIdUseCase(IReadOnlyGameRepository gameRepository, IGameCacheService? gameCacheService = null)
        {
            _gameRepository = gameRepository;
            _gameCacheService = gameCacheService ?? NullGameCacheService.Instance;
        }

        public async Task<GetGameIdOutput> Handle(GetGameIdInput input, CancellationToken cancellationToken)
        {
            var cachedGame = await _gameCacheService.GetGameByIdAsync(input.Id, cancellationToken);
            if (cachedGame is not null)
            {
                return cachedGame;
            }

            var game = await _gameRepository.GetByIdAsync(input.Id, cancellationToken);

            if (game is null)
                throw new NotFoundException(ResourceMessages.GameNotFound);

            var activePromotion = game.GetActivePromotion();
            var finalPrice = game.CalculateDiscountedPrice(activePromotion);

            var result = new GetGameIdOutput
            {
                Title = game.Title.Value,
                Description = game.Description,
                Category = game.Category.ToString(),
                OriginalPrice = game.Price.Value,
                DiscountedPrice = activePromotion != null ? finalPrice : null,
                HasActivePromotion = activePromotion != null,
            };

            await _gameCacheService.SetGameByIdAsync(input.Id, result, cancellationToken);

            return result;
        }
    }
}
