using FCG.Catalog.Domain.Catalog.Entities.Games;

namespace FCG.Catalog.Domain.Repositories.Game;

public interface IWriteOnlyPurchaseTransactionRepository
{
    Task AddAsync(PurchaseTransaction correlationId, CancellationToken cancellationToken = default);
    Task UpdateStatusAsync(Guid correlationId, string status, string? message, CancellationToken cancellationToken = default);
}