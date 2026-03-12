using FCG.Catalog.CommomTestUtilities.Builders.Audit;
using FCG.Catalog.Domain.Catalog.Entities.Audit;
using FCG.Catalog.Infrastructure.SqlServer;
using FCG.Catalog.Infrastructure.SqlServer.Audit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace FCG.Catalog.UnitTests.Infrastructure.SqlServer.Audit;

public class AuditBackgroundServiceTests
{
    private FcgCatalogDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<FcgCatalogDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new FcgCatalogDbContext(options);
    }

    private IServiceScopeFactory CreateMockScopeFactory(FcgCatalogDbContext context)
    {
        var mockScope = new Mock<IServiceScope>();
        var mockScopeFactory = new Mock<IServiceScopeFactory>();
        var mockServiceProvider = new Mock<IServiceProvider>();

        mockServiceProvider
            .Setup(x => x.GetService(typeof(FcgCatalogDbContext)))
            .Returns(context);

        mockScope.Setup(x => x.ServiceProvider).Returns(mockServiceProvider.Object);
        mockScopeFactory.Setup(x => x.CreateScope()).Returns(mockScope.Object);

        return mockScopeFactory.Object;
    }

    private AuditService CreateAuditService()
    {
        return new AuditService();
    }

    [Fact]
    public async Task ExecuteAsync_ShouldProcessAuditEntry_WhenEntryIsInChannel()
    {
        var auditService = CreateAuditService();
        var dbContext = CreateInMemoryDbContext();
        var scopeFactory = CreateMockScopeFactory(dbContext);
        var logger = new Mock<ILogger<AuditBackgroundService>>();
        var entry = new AuditEntryBuilder().Build();

        var service = new AuditBackgroundService(auditService, scopeFactory, logger.Object);

        await auditService.EnqueueAsync(entry);

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var executeTask = service.StartAsync(cts.Token);

        await Task.Delay(500);

        cts.Cancel();
        await service.StopAsync(CancellationToken.None);

        var auditTrail = await dbContext.AuditTrails.FirstOrDefaultAsync();
        auditTrail.Should().NotBeNull();
        auditTrail!.EntityName.Should().Be(entry.EntityName);
        auditTrail.Action.Should().Be(entry.Action);
        auditTrail.EntityPrimaryKey.Should().Be(entry.EntityPrimaryKey);
        auditTrail.UserId.Should().Be(entry.UserId);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldProcessMultipleEntries_WhenMultipleEntriesAreInChannel()
    {
        var auditService = CreateAuditService();
        var dbContext = CreateInMemoryDbContext();
        var scopeFactory = CreateMockScopeFactory(dbContext);
        var logger = new Mock<ILogger<AuditBackgroundService>>();
        var entries = AuditEntryBuilder.BuildList(3);

        var service = new AuditBackgroundService(auditService, scopeFactory, logger.Object);

        foreach (var entry in entries)
        {
            await auditService.EnqueueAsync(entry);
        }

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var executeTask = service.StartAsync(cts.Token);

        await Task.Delay(1000);

        cts.Cancel();
        await service.StopAsync(CancellationToken.None);

        var auditTrails = await dbContext.AuditTrails.ToListAsync();
        auditTrails.Should().HaveCount(3);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldLogWarning_WhenSaveFails()
    {
        var auditService = CreateAuditService();
        var options = new DbContextOptionsBuilder<FcgCatalogDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var dbContext = new FailingDbContext(options);
        var scopeFactory = CreateMockScopeFactory(dbContext);
        var logger = new Mock<ILogger<AuditBackgroundService>>();
        var entry = new AuditEntryBuilder().Build();

        var service = new AuditBackgroundService(auditService, scopeFactory, logger.Object);

        await auditService.EnqueueAsync(entry);

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var executeTask = service.StartAsync(cts.Token);

        await Task.Delay(1000);

        cts.Cancel();
        await service.StopAsync(CancellationToken.None);

        logger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Retry")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce());
    }

    private class FailingDbContext : FcgCatalogDbContext
    {
        public FailingDbContext(DbContextOptions<FcgCatalogDbContext> options)
            : base(options)
        {
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            throw new Exception("Simulated database failure");
        }
    }
}
