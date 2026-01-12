using FCG.Catalog.Domain.Abstractions;

namespace FCG.Catalog.Domain.Catalog.Entities.Games;

public sealed class PurchaseTransaction : BaseEntity
{
    public Guid CorrelationId => Id;
    public Guid UserId { get; private set; }
    public Guid GameId { get; private set; }
    public decimal Amount { get; private set; } 
    public string Status { get; private set; }
    public string? Message { get; private set; }

    private PurchaseTransaction() : base() { }

    public PurchaseTransaction(Guid correlationId, Guid userId, Guid gameId, decimal amount)
        : base(correlationId) 
    {
        UserId = userId;
        GameId = gameId;
        Amount = amount;
        Status = "Pending";

    }

    public void UpdateStatus(string status, string? message = null)
    {
        Status = status;
        Message = message;
        UpdatedAt = DateTime.UtcNow;
    }
}