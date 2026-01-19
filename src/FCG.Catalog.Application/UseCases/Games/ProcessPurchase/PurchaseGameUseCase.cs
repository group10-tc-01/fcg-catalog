using FCG.Catalog.Domain.Abstractions;
using FCG.Catalog.Domain.Catalog.Entities.Games;
using FCG.Catalog.Domain.Catalog.Events;
using FCG.Catalog.Domain.Exception;
using FCG.Catalog.Domain.Repositories.Game;
using FCG.Catalog.Domain.Repositories.Library;
using FCG.Catalog.Domain.Repositories.LibraryGame;
using FCG.Catalog.Domain.Repositories.Promotion;
using FCG.Catalog.Domain.Services.Repositories;
using FCG.Catalog.Infrastructure.Redis.Interface;
using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace FCG.Catalog.Application.UseCases.Games.ProcessPurchase
{
    [ExcludeFromCodeCoverage]
    public class PurchaseGameUseCase : IPurchaseGameUseCase
    {
        private readonly IReadOnlyGameRepository _readOnlyGameRepository;
        private readonly IReadOnlyLibraryGameRepository _readOnlyLibraryGameRepository;
        private readonly IReadOnlyPromotionRepository _readOnlyPromotionRepository;
        private readonly IReadOnlyLibraryRepository _readOnlyLibraryRepository;
        private readonly IWriteOnlyPurchaseTransactionRepository _writeOnlyPurchaseTransactionRepository;
        private readonly ICaching _cache;
        private readonly ICatalogLoggedUser _catalogLoggedUser;
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;

        public PurchaseGameUseCase(
            IReadOnlyGameRepository readOnlyGameRepository,
            IReadOnlyLibraryGameRepository readOnlyLibraryGameRepository,
            IReadOnlyPromotionRepository readOnlyPromotionRepository,
            IReadOnlyLibraryRepository readOnlyLibraryRepository,
            IReadOnlyPurchaseTransactionRepository readOnlyPurchaseTransactionRepository,
            IWriteOnlyPurchaseTransactionRepository writeOnlyPurchaseTransactionRepository,
            ICaching cache,
            ICatalogLoggedUser catalogLoggedUser,
            IUnitOfWork unitOfWork,
            IMediator mediator)
        {
            _readOnlyGameRepository = readOnlyGameRepository;
            _readOnlyPromotionRepository = readOnlyPromotionRepository;
            _readOnlyLibraryGameRepository = readOnlyLibraryGameRepository;
            _writeOnlyPurchaseTransactionRepository = writeOnlyPurchaseTransactionRepository;
            _readOnlyLibraryRepository = readOnlyLibraryRepository;
            _catalogLoggedUser = catalogLoggedUser;
            _unitOfWork = unitOfWork;
            _mediator = mediator;
        }

        public async Task<PurchaseGameOutput> Handle(PurchaseGameInput request, CancellationToken cancellationToken)
        {
            var correlationId = Guid.NewGuid();
            var loggedUser = await _catalogLoggedUser.GetLoggedUserAsync();
            if (loggedUser?.Id == Guid.Empty || loggedUser is null)
                throw new UnauthorizedException("User not authenticated.");

            var game = await _readOnlyGameRepository.GetByIdActiveAsync(request.Id, cancellationToken);
            if (game is null)
                throw new NotFoundException($"Game with id '{request.Id}' was not found or is inactive.");

            var library = await _readOnlyLibraryRepository.GetByUserIdAsync(loggedUser.Id, cancellationToken);
            if (library is null)
            {
                throw new NotFoundException($"Library for user '{loggedUser.Id}' was not found. Please contact support.");
            }

            var alreadyOwns = await _readOnlyLibraryGameRepository.HasGameAsync(loggedUser.Id, game.Id, cancellationToken);
            if (alreadyOwns)
                throw new DomainException($"User already owns the game: {game.Title}");

            var finalPrice = await CalculateFinalPriceAsync(game, cancellationToken);

            var transaction = new PurchaseTransaction(correlationId, loggedUser.Id, game.Id, finalPrice);

            await _writeOnlyPurchaseTransactionRepository.AddAsync(transaction, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var orderEvent = new OrderPlacedEvent(
                loggedUser.Email,
                correlationId,
                loggedUser.Id,
                game.Id,
                finalPrice,
                OccurredOn: DateTimeOffset.UtcNow
            );

            await _mediator.Publish(orderEvent, cancellationToken);

            return new PurchaseGameOutput(
                correlationId,
                transaction.Status,
                game.Title,
                Math.Round(finalPrice, 2)

            );

        }
        private async Task<decimal> CalculateFinalPriceAsync(Game game, CancellationToken cancellationToken)
        {
            var promotions = await _readOnlyPromotionRepository.GetByGameIdAsync(game.Id, cancellationToken);
            var activePromotion = promotions.Where(p => p.StartDate <= DateTime.UtcNow && p.EndDate >= DateTime.UtcNow)
                                                                    .OrderByDescending(x => x.DiscountPercentage.Value)
                                                                    .FirstOrDefault();
            if (activePromotion is null)
                return game.Price;

            var discountAmount = game.Price * activePromotion.DiscountPercentage.Value / 100;
            return game.Price - discountAmount;
        }
    }
}