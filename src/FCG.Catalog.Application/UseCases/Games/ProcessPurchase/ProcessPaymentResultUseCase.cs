using FCG.Catalog.Domain.Abstractions;
using FCG.Catalog.Domain.Catalog.Entities.LibraryGames;
using FCG.Catalog.Domain.Exception;
using FCG.Catalog.Domain.Repositories.LibraryGame;
using FCG.Catalog.Domain.Repositories.Library;
using FCG.Catalog.Infrastructure.SqlServer.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Distributed;

namespace FCG.Catalog.Application.UseCases.Games.ProcessPaymentResult
{
    public class ProcessPaymentResultUseCase : IRequestHandler<ProcessPaymentResultInput>
    {
        private readonly IReadOnlyLibraryRepository _readOnlyLibraryRepository;
        private readonly IReadOnlyLibraryGameRepository _readOnlyLibraryGameRepository;
        private readonly IWriteOnlyLibraryGameRepository _writeOnlyLibraryGameRepository;
        private readonly IDistributedCache _cache;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProcessPaymentResultUseCase> _logger;

        public ProcessPaymentResultUseCase(
            IReadOnlyLibraryRepository readOnlyLibraryRepository,
            IReadOnlyLibraryGameRepository readOnlyLibraryGameRepository,
            IWriteOnlyLibraryGameRepository writeOnlyLibraryGameRepository,
            IUnitOfWork unitOfWork,
            IDistributedCache cache,
            ILogger<ProcessPaymentResultUseCase> logger)
        {
            _readOnlyLibraryRepository = readOnlyLibraryRepository;
            _readOnlyLibraryGameRepository = readOnlyLibraryGameRepository;
            _writeOnlyLibraryGameRepository = writeOnlyLibraryGameRepository;
            _cache = cache;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(ProcessPaymentResultInput request, CancellationToken cancellationToken)
        {
            if (!request.IsApproved)
            {
                _logger.LogWarning("Pagamento recusado para User {UserId}. Jogo não será liberado.", request.UserId);
                return;
            }

            var alreadyOwns = await _readOnlyLibraryGameRepository.HasGameAsync(request.UserId, request.GameId, cancellationToken);
            if (alreadyOwns)
            {
                _logger.LogInformation("Usuario {UserId} já possui o jogo {GameId}.", request.UserId, request.GameId);
                return;
            }

            var library = await _readOnlyLibraryRepository.GetByUserIdAsync(request.UserId, cancellationToken);

            if (library == null)
            {
                _logger.LogError("Biblioteca não encontrada para o usuário {UserId}. Impossível adicionar jogo.", request.UserId);
                throw new NotFoundException($"Library not found for user {request.UserId}");
            }

            var libraryGame = LibraryGame.Create(library.Id, request.GameId, request.Amount);

            await _writeOnlyLibraryGameRepository.AddAsync(libraryGame, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _cache.RemoveAsync($"library:{request.UserId}", cancellationToken);

            _logger.LogInformation("Jogo {GameId} liberado com sucesso na biblioteca {LibraryId} (User {UserId}).", request.GameId, library.Id, request.UserId);
        }
    }
}