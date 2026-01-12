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
        public GetGameIdUseCase(IReadOnlyGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }
        public async Task<GetGameIdOutput?> Handle(GetGameIdInput input, CancellationToken cancellationToken)
        {
            var game = await _gameRepository.GetByIdAsync(input.Id, cancellationToken);

            if (game is null)
                throw new NotFoundException(ResourceMessages.GameNotFound);

            var activePromotion = game.GetActivePromotion();
            var finalPrice = game.CalculateDiscountedPrice(activePromotion);

            return new GetGameIdOutput
            {
                Title = game.Title.Value,
                Description = game.Description,
                Category = game.Category.ToString(),
                OriginalPrice = game.Price.Value,
                DiscountedPrice = activePromotion != null ? finalPrice : null,
                HasActivePromotion = activePromotion != null,
            };
        }
    }
}
