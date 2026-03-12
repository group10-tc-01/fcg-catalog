using FCG.Catalog.CommomTestUtilities.Builders.Games;
using FCG.Catalog.Domain.Catalog.Entities.Audit;
using FCG.Catalog.Domain.Catalog.Entities.Games;
using FCG.Catalog.Domain.Catalog.ValueObjects;
using FCG.Catalog.Domain.Enum;
using FCG.Catalog.Domain.Interfaces;
using FCG.Catalog.Infrastructure.SqlServer;
using FCG.Catalog.Infrastructure.SqlServer.Persistence.Interceptors;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace FCG.Catalog.UnitTests.Infrastructure.SqlServer.Audit;

public class AuditingInterceptorTests : IDisposable
{
    private readonly Mock<ICurrentSessionProvider> _mockSessionProvider;
    private readonly Mock<IAuditService> _mockAuditService;
    private readonly AuditingInterceptor _interceptor;
    private readonly List<AuditEntry> _capturedEntries;

    public AuditingInterceptorTests()
    {
        _mockSessionProvider = new Mock<ICurrentSessionProvider>();
        _mockAuditService = new Mock<IAuditService>();
        _capturedEntries = new List<AuditEntry>();

        _mockAuditService
            .Setup(x => x.EnqueueAsync(It.IsAny<AuditEntry>(), It.IsAny<CancellationToken>()))
            .Callback<AuditEntry, CancellationToken>((entry, _) => _capturedEntries.Add(entry))
            .Returns(ValueTask.CompletedTask);

        _interceptor = new AuditingInterceptor(_mockSessionProvider.Object, _mockAuditService.Object);
    }

    public void Dispose()
    {
        _capturedEntries.Clear();
    }

    private FcgCatalogDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<FcgCatalogDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .AddInterceptors(_interceptor)
            .Options;

        return new FcgCatalogDbContext(options);
    }

    [Fact]
    public async Task SavingChangesAsync_ShouldCaptureCreateAction_WhenEntityIsAdded()
    {
        var dbContext = CreateDbContext();
        var userId = Guid.NewGuid();
        var userName = "testuser@example.com";
        var correlationId = Guid.NewGuid();

        _mockSessionProvider.Setup(x => x.GetUserId()).Returns(userId);
        _mockSessionProvider.Setup(x => x.GetUserName()).Returns(userName);
        _mockSessionProvider.Setup(x => x.GetCorrelationId()).Returns(correlationId);

        var game = new GameBuilder().Build();

        dbContext.Games.Add(game);
        await dbContext.SaveChangesAsync();

        var gameAuditEntry = _capturedEntries.FirstOrDefault(e => e.EntityName == "Game");

        gameAuditEntry.Should().NotBeNull();
        gameAuditEntry!.Action.Should().Be("Create");
        gameAuditEntry.EntityPrimaryKey.Should().Be(game.Id.ToString());
        gameAuditEntry.NewValue.Should().NotBeNull();
        gameAuditEntry.OldValue.Should().BeNull();
        gameAuditEntry.UserId.Should().Be(userId);
        gameAuditEntry.UserName.Should().Be(userName);
        gameAuditEntry.CorrelationId.Should().Be(correlationId);
    }

    [Fact]
    public async Task SavingChangesAsync_ShouldNotCaptureAuditTrail_WhenAuditTrailEntityIsModified()
    {
        var dbContext = CreateDbContext();

        _mockSessionProvider.Setup(x => x.GetUserId()).Returns(Guid.NewGuid());
        _mockSessionProvider.Setup(x => x.GetUserName()).Returns("testuser@example.com");
        _mockSessionProvider.Setup(x => x.GetCorrelationId()).Returns(Guid.NewGuid());

        var auditTrail = new AuditTrail(
            "TestEntity",
            "Create",
            Guid.NewGuid().ToString(),
            null,
            "{\"value\":\"test\"}",
            Guid.NewGuid(),
            "testuser",
            Guid.NewGuid(),
            null);

        dbContext.AuditTrails.Add(auditTrail);
        await dbContext.SaveChangesAsync();

        _capturedEntries.Should().BeEmpty();
    }

    [Fact]
    public async Task SavingChangesAsync_ShouldUseEmptyGuid_WhenUserIdIsNull()
    {
        var dbContext = CreateDbContext();

        _mockSessionProvider.Setup(x => x.GetUserId()).Returns((Guid?)null);
        _mockSessionProvider.Setup(x => x.GetUserName()).Returns((string?)null);
        _mockSessionProvider.Setup(x => x.GetCorrelationId()).Returns((Guid?)null);

        var game = new GameBuilder().Build();

        dbContext.Games.Add(game);
        await dbContext.SaveChangesAsync();

        var gameAuditEntry = _capturedEntries.FirstOrDefault(e => e.EntityName == "Game");

        gameAuditEntry.Should().NotBeNull();
        gameAuditEntry!.UserId.Should().Be(Guid.Empty);
        gameAuditEntry.UserName.Should().BeNull();
        gameAuditEntry.CorrelationId.Should().BeNull();
    }

    [Fact]
    public void Constructor_ShouldInitialize_WithSessionProviderAndAuditService()
    {
        var sessionProvider = new Mock<ICurrentSessionProvider>();
        var auditService = new Mock<IAuditService>();

        var interceptor = new AuditingInterceptor(sessionProvider.Object, auditService.Object);

        interceptor.Should().NotBeNull();
    }

    [Fact]
    public async Task SavingChangesAsync_ShouldCaptureAtLeastOneGameEntity_WhenGamesAreAdded()
    {
        var dbContext = CreateDbContext();
        var userId = Guid.NewGuid();

        _mockSessionProvider.Setup(x => x.GetUserId()).Returns(userId);
        _mockSessionProvider.Setup(x => x.GetUserName()).Returns("testuser@example.com");
        _mockSessionProvider.Setup(x => x.GetCorrelationId()).Returns(Guid.NewGuid());

        var game1 = new GameBuilder().Build();
        var game2 = new GameBuilder().Build();

        dbContext.Games.AddRange(game1, game2);
        await dbContext.SaveChangesAsync();

        var gameEntries = _capturedEntries.Where(e => e.EntityName == "Game").ToList();

        gameEntries.Count.Should().BeGreaterThanOrEqualTo(2);
        gameEntries.All(e => e.Action == "Create").Should().BeTrue();
    }
}
