namespace FCG.Catalog.Domain.Interfaces;

public interface IAuditService
{
    ValueTask EnqueueAsync<T>(T entry, CancellationToken cancellationToken = default) where T : class;
}

public interface ICurrentSessionProvider
{
    Guid? GetUserId();
    Guid? GetCorrelationId();
    string? GetUserName();
}
