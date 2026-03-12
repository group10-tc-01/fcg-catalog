namespace FCG.Catalog.Domain.Catalog.Entities.Audit;

public sealed record AuditEntry(
    string EntityName,
    string Action,
    string EntityPrimaryKey,
    string? OldValue,
    string? NewValue,
    DateTime OccurredAt,
    Guid UserId,
    string? UserName,
    Guid? CorrelationId
);
