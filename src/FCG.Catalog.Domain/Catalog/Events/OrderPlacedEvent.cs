using FCG.Catalog.Domain.Abstractions;

namespace FCG.Catalog.Domain.Catalog.Events
{
    public record OrderPlacedEvent(
        Guid CorrelationId,
        Guid UserId,
        Guid GameId,
        decimal Amount,
        DateTimeOffset OccurredOn
    ) : IDomainEvent;
}