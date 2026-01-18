using FCG.Catalog.Domain.Catalog.Entities.Games;
using FCG.Catalog.Domain.Repositories.Game;
using Microsoft.EntityFrameworkCore;

namespace FCG.Catalog.Infrastructure.SqlServer.Repositories;

public class PurchaseTransactionRepository : IReadOnlyPurchaseTransactionRepository,
    IWriteOnlyPurchaseTransactionRepository
{
    private readonly FcgCatalogDbContext _fcgDbContext;

    public PurchaseTransactionRepository(FcgCatalogDbContext fcgDbContext)
    {
        _fcgDbContext = fcgDbContext;
    }

    public async Task<PurchaseTransaction?> GetByCorrelationIdAsync(Guid correlationId,
        CancellationToken cancellationToken)
    {
        return await _fcgDbContext.PurchaseTransactions
            .FirstOrDefaultAsync(t => t.Id == correlationId, cancellationToken);
    }

    public async Task AddAsync(PurchaseTransaction transaction, CancellationToken cancellationToken = default)
    {
        await _fcgDbContext.PurchaseTransactions.AddAsync(transaction, cancellationToken);
    }

    public async Task UpdateStatusAsync(
        Guid correlationId,
        string status,
        string? message,
        CancellationToken cancellationToken = default)
    {
        var transaction = await _fcgDbContext.PurchaseTransactions
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.Id == correlationId, cancellationToken);
        if (transaction != null)
        {
            transaction.UpdateStatus(status, message);
            _fcgDbContext.PurchaseTransactions.Update(transaction);
        }
    }
}