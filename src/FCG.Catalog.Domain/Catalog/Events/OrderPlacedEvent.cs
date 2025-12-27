using FCG.Catalog.Domain.Abstractions;

namespace FCG.Catalog.Domain.Catalog.Events
{
    public record OrderPlacedEvent(Guid LibraryId, Guid GameId, DateTime OrderDate) : IDomainEvent;
}
