using FCG.Catalog.Domain.Catalog.Entities.Audit;
using FCG.Catalog.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text.Json;

namespace FCG.Catalog.Infrastructure.SqlServer.Persistence.Interceptors;

public sealed class AuditingInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentSessionProvider _currentSessionProvider;
    private readonly IAuditService _auditService;

    public AuditingInterceptor(ICurrentSessionProvider currentSessionProvider, IAuditService auditService)
    {
        _currentSessionProvider = currentSessionProvider;
        
        _auditService = auditService;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        ProcessSavingChanges(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ProcessSavingChanges(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void ProcessSavingChanges(DbContext? context)
    {
        if (context == null) return;

        var entries = context.ChangeTracker
            .Entries()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted
                        && e.Entity.GetType() != typeof(AuditTrail));

        foreach (var entry in entries)
        {
            var auditEntry = CreateAuditEntry(entry);
            if (auditEntry != null)
            {
                _auditService.EnqueueAsync(auditEntry).AsTask().Wait();
            }
        }
    }

    private AuditEntry? CreateAuditEntry(EntityEntry entry)
    {
        var entityType = entry.Entity.GetType();
        var entityName = entityType.Name;
        var primaryKey = GetPrimaryKeyValue(entry);
        var action = GetAction(entry.State);
        var occurredAt = DateTime.UtcNow;

        var userId = _currentSessionProvider.GetUserId() ?? Guid.Empty;
        var userName = _currentSessionProvider.GetUserName();
        var correlationId = _currentSessionProvider.GetCorrelationId();

        string? oldValue = null;
        string? newValue = null;

        switch (entry.State)
        {
            case EntityState.Added:
                newValue = SerializeValues(entry.CurrentValues);
                break;

            case EntityState.Modified:
                oldValue = SerializeValues(entry.OriginalValues);
                newValue = SerializeValues(entry.CurrentValues);
                break;

            case EntityState.Deleted:
                oldValue = SerializeValues(entry.OriginalValues);
                break;
        }

        return new AuditEntry(
            entityName,
            action,
            primaryKey,
            oldValue,
            newValue,
            occurredAt,
            userId,
            userName,
            correlationId);
    }

    private static string GetAction(EntityState state) => state switch
    {
        EntityState.Added => AuditTrailType.Create.ToString(),
        EntityState.Modified => AuditTrailType.Update.ToString(),
        EntityState.Deleted => AuditTrailType.Delete.ToString(),
        _ => string.Empty
    };

    private static string GetPrimaryKeyValue(EntityEntry entry)
    {
        var key = entry.Metadata.FindPrimaryKey();
        if (key == null) return string.Empty;

        var primaryKeyValue = entry.Property(key.Properties.First().Name).CurrentValue;
        return primaryKeyValue?.ToString() ?? string.Empty;
    }

    private static string? SerializeValues(PropertyValues? values)
    {
        if (values == null) return null;

        var dictionary = values.Properties
            .ToDictionary(
                p => p.Name,
                p => values[p.Name]?.GetType().IsClass == true
                    ? JsonSerializer.Serialize(values[p.Name])
                    : values[p.Name]?.ToString());

        return JsonSerializer.Serialize(dictionary);
    }
}
