using FCG.Catalog.Application.UseCases.Games.Purchase;
using FCG.Catalog.Domain.Abstractions;
using FCG.Catalog.Domain.Catalog.Entity.Games;
using FCG.Catalog.Domain.Exception;
using FCG.Catalog.Domain.Repositories.Game;
using FCG.Catalog.Domain.Repositories.Library;
using FCG.Catalog.Domain.Services.Repositories;
using FCG.Domain.Repositories.PromotionRepository;
using System.Diagnostics.CodeAnalysis;

namespace FCG.Catalog.Application.UseCases.Games.ProcessPurchase
{
    [ExcludeFromCodeCoverage]
    public class PurchaseGameUseCase : IPurchaseGameUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReadOnlyGameRepository _readOnlyGameRepository;
        private readonly IReadOnlyPromotionRepository _readOnlyPromotionRepository;
        private readonly IReadOnlyLibraryRepository _readOnlyLibraryRepository;
        private readonly ICatalogLoggedUser _catalogLoggedUser;

        public PurchaseGameUseCase(IUnitOfWork unitOfWork, IReadOnlyGameRepository readOnlyGameRepository,
            IReadOnlyPromotionRepository readOnlyPromotionRepository,
            IReadOnlyLibraryRepository readOnlyLibraryRepository,
            ICatalogLoggedUser catalogLoggedUser)
        {
            _unitOfWork = unitOfWork;
            _readOnlyGameRepository = readOnlyGameRepository;
            _readOnlyPromotionRepository = readOnlyPromotionRepository;
            _readOnlyLibraryRepository = readOnlyLibraryRepository;
            _catalogLoggedUser = catalogLoggedUser;
        }

        public async Task<PurchaseGameOutput> Handle(PurchaseGameInput request, CancellationToken cancellationToken)
        {
            var game = await _readOnlyGameRepository.GetByIdActiveAsync(request.Id, cancellationToken);
            if (game is null)
                throw new NotFoundException($"Game with id '{request.Id}' was not found.");

            var loggedUser = await _catalogLoggedUser.GetLoggedUserAsync();
            if (loggedUser?.Id == Guid.Empty || loggedUser is null)
                throw new UnauthorizedException("User not authenticated.");

            var library = await _readOnlyLibraryRepository.GetByUserIdAsync(loggedUser.Id, cancellationToken);
            if (library is null)
                throw new NotFoundException($"Library for user '{loggedUser.Id}' was not found.");

            var gameAlreadyOwned = library.LibraryGames?.Any(lg => lg.GameId == game.Id) ?? false;
            if (gameAlreadyOwned)
                throw new DomainException($"User already owns the game: {game.Title}");

            var finalPrice = await CalculateFinalPriceAsync(game, cancellationToken);

            library.AddGame(game.Id, finalPrice);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new PurchaseGameOutput(game.Title, Math.Round(game.Price, 2), Math.Round(finalPrice, 2));
        }

        private async Task<decimal> CalculateFinalPriceAsync(Game game, CancellationToken cancellationToken)
        {
            var promotions = await _readOnlyPromotionRepository.GetByGameIdAsync(game.Id, cancellationToken);
            var activePromotion = promotions?.FirstOrDefault(p => p.StartDate <= DateTime.UtcNow && p.EndDate >= DateTime.UtcNow);

            if (activePromotion is null)
                return game.Price;

            var discountAmount = game.Price * activePromotion.DiscountPercentage.Value / 100;
            return game.Price - discountAmount;
        }
    }
}
