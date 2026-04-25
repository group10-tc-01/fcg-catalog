using FCG.Catalog.Application.Abstractions.Caching;
using FCG.Catalog.Domain.Abstractions;
using FCG.Catalog.Domain.Exception;
using FCG.Catalog.Domain.Repositories.Game;
using FCG.Catalog.Domain.Repositories.Promotion;
using FCG.Catalog.Messages;
using MediatR;

namespace FCG.Catalog.Application.UseCases.Games.Delete
{
    public class DeleteGameUseCase : IDeleteGameUseCase
    {
        private readonly IReadOnlyGameRepository _gameRepository;
        private readonly IReadOnlyPromotionRepository _readOnlyPromotionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGameCacheService _gameCacheService;

        public DeleteGameUseCase(
            IReadOnlyGameRepository gameRepository,
            IReadOnlyPromotionRepository readOnlyPromotionRepository,
            IUnitOfWork unitOfWork,
            IGameCacheService? gameCacheService = null)
        {
            _gameRepository = gameRepository;
            _readOnlyPromotionRepository = readOnlyPromotionRepository;
            _unitOfWork = unitOfWork;
            _gameCacheService = gameCacheService ?? NullGameCacheService.Instance;
        }

        public async Task<Unit> Handle(DeleteGameInput request, CancellationToken cancellationToken)
        {
            var game = await _gameRepository.GetByIdAsync(request.Id, cancellationToken);
            var promotions = await _readOnlyPromotionRepository.GetByGameIdAsync(request.Id, cancellationToken);

            if (game is null)
            {
                throw new NotFoundException(ResourceMessages.GameNotFound);
            }
            if (promotions.Any())
            {
                throw new DomainException(ResourceMessages.GameWithPromotion);
            }
            
            await _gameRepository.Delete(game, cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _gameCacheService.InvalidateGameByIdAsync(request.Id, cancellationToken);
            await _gameCacheService.InvalidateGameListAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
