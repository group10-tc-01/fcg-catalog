using FCG.Catalog.Domain.Abstractions;
using MediatR;

namespace FCG.Catalog.Domain.Catalog.Events
{
    public sealed record GameCreatedEvent(
        Guid GameId,
        string Title,
        string Description,
        decimal Price,
        string Category,
        DateTime CreatedAt
    ) : IDomainEvent;
}
