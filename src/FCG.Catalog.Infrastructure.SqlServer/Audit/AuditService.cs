using FCG.Catalog.Domain.Catalog.Entities.Audit;
using FCG.Catalog.Domain.Interfaces;
using System.Threading.Channels;

namespace FCG.Catalog.Infrastructure.SqlServer.Audit;

public sealed class AuditService : IAuditService
{
    private const int MaxQueueSize = 1000;
    private readonly Channel<AuditEntry> _channel;

    public AuditService()
    {
        var options = new BoundedChannelOptions(MaxQueueSize)
        {
            FullMode = BoundedChannelFullMode.Wait
        };
        _channel = Channel.CreateBounded<AuditEntry>(options);
    }

    public ChannelReader<AuditEntry> Reader => _channel.Reader;

    public async ValueTask EnqueueAsync(AuditEntry entry, CancellationToken cancellationToken = default)
    {
        await _channel.Writer.WriteAsync(entry, cancellationToken);
    }

    async ValueTask IAuditService.EnqueueAsync<T>(T entry, CancellationToken cancellationToken)
    {
        if (entry is AuditEntry auditEntry)
        {
            await _channel.Writer.WriteAsync(auditEntry, cancellationToken);
        }
    }
}
