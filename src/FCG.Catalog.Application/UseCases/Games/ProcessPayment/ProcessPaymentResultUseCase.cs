using FCG.Catalog.Domain.Abstractions;
using FCG.Catalog.Domain.Catalog.Entities.LibraryGames;
using FCG.Catalog.Domain.Exception;
using FCG.Catalog.Domain.Repositories.Game;
using FCG.Catalog.Domain.Repositories.Library;
using FCG.Catalog.Domain.Repositories.LibraryGame;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace FCG.Catalog.Application.UseCases.Games.ProcessPayment
{
    public class ProcessPaymentResultUseCase : IRequestHandler<ProcessPaymentResultInput>
    {
        private readonly IReadOnlyLibraryRepository _readOnlyLibraryRepository;
        private readonly IReadOnlyLibraryGameRepository _readOnlyLibraryGameRepository;
        private readonly IWriteOnlyLibraryGameRepository _writeOnlyLibraryGameRepository;
        private readonly IWriteOnlyPurchaseTransactionRepository _writeOnlyPurchaseTransactionRepository;
        private readonly IDistributedCache _cache;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProcessPaymentResultUseCase> _logger;

        public ProcessPaymentResultUseCase(
            IReadOnlyLibraryRepository readOnlyLibraryRepository,
            IReadOnlyLibraryGameRepository readOnlyLibraryGameRepository,
            IWriteOnlyLibraryGameRepository writeOnlyLibraryGameRepository,
            IWriteOnlyPurchaseTransactionRepository writeOnlyPurchaseTransactionRepository,
            IUnitOfWork unitOfWork,
            IDistributedCache cache,
            ILogger<ProcessPaymentResultUseCase> logger)
        {
            _readOnlyLibraryRepository = readOnlyLibraryRepository;
            _readOnlyLibraryGameRepository = readOnlyLibraryGameRepository;
            _writeOnlyLibraryGameRepository = writeOnlyLibraryGameRepository;
            _writeOnlyPurchaseTransactionRepository = writeOnlyPurchaseTransactionRepository;
            _cache = cache;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(ProcessPaymentResultInput request, CancellationToken cancellationToken)
        {
            string statusFinal = request.IsApproved ? "Completed" : "Rejected";
            string? mensagem = request.IsApproved ? null : "Payment rejected";

            await _writeOnlyPurchaseTransactionRepository.UpdateStatusAsync(request.CorrelationId, statusFinal, mensagem, cancellationToken);

            if (request.IsApproved)
            {
                var alreadyOwns = await _readOnlyLibraryGameRepository.HasGameAsync(request.UserId, request.GameId, cancellationToken);
                if (!alreadyOwns)
                {
                    var library = await _readOnlyLibraryRepository.GetByUserIdAsync(request.UserId, cancellationToken);
                    if (library == null) throw new NotFoundException($"Library not found for user {request.UserId}");

                    var libraryGame = LibraryGame.Create(library.Id, request.GameId, request.Amount);
                    await _writeOnlyLibraryGameRepository.AddAsync(libraryGame, cancellationToken);
                }
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _cache.RemoveAsync($"library:{request.UserId}", cancellationToken);
        }
    }
}