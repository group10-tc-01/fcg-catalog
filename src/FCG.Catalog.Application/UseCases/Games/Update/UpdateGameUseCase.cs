using FCG.Catalog.Application.Abstractions.Caching;
using FCG.Catalog.Domain.Abstractions;
using FCG.Catalog.Domain.Enum;
using FCG.Catalog.Domain.Exception;
using FCG.Catalog.Domain.Repositories.Game;
using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace FCG.Catalog.Application.UseCases.Games.Update
{
    [ExcludeFromCodeCoverage]
    public class UpdateGameUseCase : IRequestHandler<UpdateGameInput, UpdateGameOutput>
    {
        private readonly IReadOnlyGameRepository _readOnlyGameRepository;
        private readonly IWriteOnlyGameRepository _writeOnlyGameRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGameCacheService _gameCacheService;

        public UpdateGameUseCase(IReadOnlyGameRepository readOnlyGameRepository, IWriteOnlyGameRepository writeOnlyGameRepository,
                                IUnitOfWork unitOfWork,
                                IGameCacheService? gameCacheService = null)
        {
            _readOnlyGameRepository = readOnlyGameRepository;
            _writeOnlyGameRepository = writeOnlyGameRepository;
            _unitOfWork = unitOfWork;
            _gameCacheService = gameCacheService ?? NullGameCacheService.Instance;
        }

        public async Task<UpdateGameOutput> Handle(UpdateGameInput request, CancellationToken cancellationToken)
        {
            var game = await _readOnlyGameRepository.GetByIdAsync(request.Id, cancellationToken);

            if (game is null)
            {
                throw new NotFoundException($"Game id '{request.Id}' not found.");
            }

            if (!Enum.IsDefined(typeof(GameCategory), request.Category))
            {
                throw new DomainException($"Invalid category: '{request.Category}'. Available categories are: Action, Adventure, RPG...");
            }

            game.Update(
                request.Title,
                request.Description,
                request.Price,
                request.Category,
                DateTime.UtcNow
            );

            _writeOnlyGameRepository.Update(game);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _gameCacheService.InvalidateGameByIdAsync(request.Id, cancellationToken);
            await _gameCacheService.InvalidateGameListAsync(cancellationToken);

            return new UpdateGameOutput(game.Id, game.Title, game.Price.Value, game.Description, game.Category.ToString(), game.UpdatedAt);
        }
    }
}
