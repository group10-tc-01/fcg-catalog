using FCG.Catalog.Domain.Catalog.Entities.Games;

namespace FCG.Catalog.Domain.Repositories.Game;

public interface IReadOnlyPurchaseTransactionRepository
{
    Task<PurchaseTransaction?> GetByCorrelationIdAsync(Guid correlationId, CancellationToken ct);
}