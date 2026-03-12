using FCG.Catalog.Domain.Catalog.Entities.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;

namespace FCG.Catalog.Infrastructure.SqlServer.Audit;

public sealed class AuditBackgroundService : BackgroundService
{
    private readonly AuditService _auditService;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AuditBackgroundService> _logger;

    public AuditBackgroundService(
        AuditService auditService,
        IServiceScopeFactory scopeFactory,
        ILogger<AuditBackgroundService> logger)
    {
        _auditService = auditService;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Audit Background Service started");

        await foreach (var entry in _auditService.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                await ProcessWithRetryAsync(entry, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process audit entry for {EntityName} {EntityPrimaryKey} after all retries",
                    entry.EntityName, entry.EntityPrimaryKey);
            }
        }
    }

    private async Task ProcessWithRetryAsync(AuditEntry entry, CancellationToken cancellationToken)
    {
        var policy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                3,
                attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                (exception, timeSpan, retryCount, context) =>
                {
                    _logger.LogWarning(exception,
                        "Retry {RetryCount} for audit entry {EntityName} {EntityPrimaryKey} after {Delay}s",
                        retryCount, entry.EntityName, entry.EntityPrimaryKey, timeSpan.TotalSeconds);
                });

        await policy.ExecuteAsync(async () =>
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<FcgCatalogDbContext>();

            var auditTrail = new AuditTrail(
                entry.EntityName,
                entry.Action,
                entry.EntityPrimaryKey,
                entry.OldValue,
                entry.NewValue,
                entry.UserId,
                entry.UserName,
                entry.CorrelationId,
                null);

            dbContext.AuditTrails.Add(auditTrail);
            await dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogDebug("Audit entry saved for {EntityName} {EntityPrimaryKey}",
                entry.EntityName, entry.EntityPrimaryKey);
        });
    }
}
