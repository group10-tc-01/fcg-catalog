using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FCG.Catalog.Domain;
using FCG.Catalog.Domain.Exception;
using FCG.Catalog.Domain.Repositories.Game;
using FCG.Domain.Repositories.LibraryRepository;
using FCG.Domain.Repositories.PromotionRepository;
using FCG.Catalog.Domain.Services.Repositories;

namespace FCG.Catalog.Application.UseCases.Games.Purchase
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
            var game = await _readOnlyGameRepository.GetByIdAsync(request.Id, cancellationToken);


            if (game is null)
            {
                throw new NotFoundException($"Game with id '{request.Id}' was not found.");
            }

            var promotions = await _readOnlyPromotionRepository.GetByGameIdAsync(game.Id, cancellationToken);

            var loggedUser = await _catalogLoggedUser.GetLoggedUserAsync();
            if (loggedUser == null || loggedUser.Id == Guid.Empty)
                throw new UnauthorizedException("User not authenticated.");
            Guid userId = loggedUser.Id;

            var library = await _readOnlyLibraryRepository.GetByUserIdAsync(userId, cancellationToken);

            var activePromotion = promotions?.FirstOrDefault(p => p.StartDate <= DateTime.UtcNow && p.EndDate >= DateTime.UtcNow);
            var finalPrice = game.Price;

            if (activePromotion is not null)
            {
                var discountAmount = (game.Price * activePromotion.DiscountPercentage.Value) / 100;
                finalPrice = game.Price - discountAmount;
            }

            if (library is null)
            {
                throw new NotFoundException($"Library for user '{userId}' was not found.");
            }
            var gameAlreadyOwned = library?.LibraryGames?.Any(lg => lg.GameId == game.Id) ?? false;

            if (gameAlreadyOwned)
            {
                throw new DomainException($"User already owns the game: {game.Title}");
            }
            library!.AddGame(game.Id, finalPrice);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new PurchaseGameOutput(game.Title, Math.Round(game.Price, 2), Math.Round(finalPrice, 2));

        }
    }
}
