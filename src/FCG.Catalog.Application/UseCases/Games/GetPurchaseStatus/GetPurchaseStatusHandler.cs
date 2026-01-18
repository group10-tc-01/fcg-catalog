using FCG.Catalog.Domain.Repositories.Game;
using FCG.Catalog.Infrastructure.Redis.Interface;
using MediatR;

namespace FCG.Catalog.Application.UseCases.Games.GetPurchaseStatus;

public class GetPurchaseStatusHandler : IRequestHandler<GetPurchaseStatusInput, PurchaseStatusOutput>
{
    private readonly IReadOnlyPurchaseTransactionRepository _readOnlyPurchaseTransactionRepository;

    public GetPurchaseStatusHandler(IReadOnlyPurchaseTransactionRepository readOnlyPurchaseTransactionRepository, ICaching cache)
    {
        _readOnlyPurchaseTransactionRepository = readOnlyPurchaseTransactionRepository;
    }

    public async Task<PurchaseStatusOutput> Handle(GetPurchaseStatusInput request, CancellationToken correlationId)
    {
        var transaction =
            await _readOnlyPurchaseTransactionRepository.GetByCorrelationIdAsync(request.CorrelationId, correlationId);

        if (transaction is null)
            return new PurchaseStatusOutput(request.CorrelationId, "NotFound", "Transação não encontrada.");

        return new PurchaseStatusOutput(
            transaction.CorrelationId,
            transaction.Status,
            transaction.Message
        );
    }
}