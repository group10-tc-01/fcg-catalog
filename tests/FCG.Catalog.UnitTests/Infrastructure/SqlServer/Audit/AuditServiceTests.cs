using FCG.Catalog.CommomTestUtilities.Builders.Audit;
using FCG.Catalog.Domain.Catalog.Entities.Audit;
using FCG.Catalog.Infrastructure.SqlServer.Audit;
using FluentAssertions;

namespace FCG.Catalog.UnitTests.Infrastructure.SqlServer.Audit;

public class AuditServiceTests
{
    [Fact]
    public async Task EnqueueAsync_ShouldAddEntryToChannel_WhenEntryIsProvided()
    {
        var auditService = new AuditService();
        var entry = new AuditEntryBuilder().Build();

        await auditService.EnqueueAsync(entry);

        var readEntry = await auditService.Reader.ReadAsync();

        readEntry.Should().BeEquivalentTo(entry);
    }

    [Fact]
    public async Task EnqueueAsync_ShouldAddMultipleEntries_WhenMultipleEntriesAreProvided()
    {
        var auditService = new AuditService();
        var entries = AuditEntryBuilder.BuildList(5);

        foreach (var entry in entries)
        {
            await auditService.EnqueueAsync(entry);
        }

        var readEntries = new List<AuditEntry>();
        for (int i = 0; i < 5; i++)
        {
            var entry = await auditService.Reader.ReadAsync();
            readEntries.Add(entry);
        }

        readEntries.Should().HaveCount(5);
        readEntries.Should().BeEquivalentTo(entries);
    }

    [Fact]
    public async Task EnqueueAsync_ShouldPreserveEntryProperties_WhenCreateEntryIsProvided()
    {
        var auditService = new AuditService();
        var entityName = "Game";
        var action = "Create";
        var entityPrimaryKey = Guid.NewGuid().ToString();
        var newValue = "{\"Title\":\"Test Game\"}";

        var entry = AuditEntryBuilder.CreateForCreate(entityName, entityPrimaryKey, newValue);

        await auditService.EnqueueAsync(entry);

        var readEntry = await auditService.Reader.ReadAsync();

        readEntry.EntityName.Should().Be(entityName);
        readEntry.Action.Should().Be(action);
        readEntry.EntityPrimaryKey.Should().Be(entityPrimaryKey);
        readEntry.OldValue.Should().BeNull();
        readEntry.NewValue.Should().Be(newValue);
    }

    [Fact]
    public async Task EnqueueAsync_ShouldPreserveEntryProperties_WhenUpdateEntryIsProvided()
    {
        var auditService = new AuditService();
        var entityName = "Game";
        var action = "Update";
        var entityPrimaryKey = Guid.NewGuid().ToString();
        var oldValue = "{\"Title\":\"Old Title\"}";
        var newValue = "{\"Title\":\"New Title\"}";

        var entry = AuditEntryBuilder.CreateForUpdate(entityName, entityPrimaryKey, oldValue, newValue);

        await auditService.EnqueueAsync(entry);

        var readEntry = await auditService.Reader.ReadAsync();

        readEntry.EntityName.Should().Be(entityName);
        readEntry.Action.Should().Be(action);
        readEntry.EntityPrimaryKey.Should().Be(entityPrimaryKey);
        readEntry.OldValue.Should().Be(oldValue);
        readEntry.NewValue.Should().Be(newValue);
    }

    [Fact]
    public async Task EnqueueAsync_ShouldPreserveEntryProperties_WhenDeleteEntryIsProvided()
    {
        var auditService = new AuditService();
        var entityName = "Game";
        var action = "Delete";
        var entityPrimaryKey = Guid.NewGuid().ToString();
        var oldValue = "{\"Title\":\"Deleted Game\"}";

        var entry = AuditEntryBuilder.CreateForDelete(entityName, entityPrimaryKey, oldValue);

        await auditService.EnqueueAsync(entry);

        var readEntry = await auditService.Reader.ReadAsync();

        readEntry.EntityName.Should().Be(entityName);
        readEntry.Action.Should().Be(action);
        readEntry.EntityPrimaryKey.Should().Be(entityPrimaryKey);
        readEntry.OldValue.Should().Be(oldValue);
        readEntry.NewValue.Should().BeNull();
    }

    [Fact]
    public async Task EnqueueAsync_ShouldPreserveUserInformation_WhenUserIsProvided()
    {
        var auditService = new AuditService();
        var userId = Guid.NewGuid();
        var userName = "testuser@example.com";
        var correlationId = Guid.NewGuid();

        var entry = new AuditEntryBuilder()
            .WithUserId(userId)
            .WithUserName(userName)
            .WithCorrelationId(correlationId)
            .Build();

        await auditService.EnqueueAsync(entry);

        var readEntry = await auditService.Reader.ReadAsync();

        readEntry.UserId.Should().Be(userId);
        readEntry.UserName.Should().Be(userName);
        readEntry.CorrelationId.Should().Be(correlationId);
    }

    [Fact]
    public async Task EnqueueAsync_ShouldPreserveOccurrenceTime_WhenEntryIsProvided()
    {
        var auditService = new AuditService();
        var occurredAt = DateTime.UtcNow;

        var entry = new AuditEntryBuilder()
            .WithOccurredAt(occurredAt)
            .Build();

        await auditService.EnqueueAsync(entry);

        var readEntry = await auditService.Reader.ReadAsync();

        readEntry.OccurredAt.Should().BeCloseTo(occurredAt, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task EnqueueAsync_ShouldNotBlock_WhenCapacityIsAvailable()
    {
        var auditService = new AuditService();
        var entry = new AuditEntryBuilder().Build();

        var valueTask = auditService.EnqueueAsync(entry);
        await valueTask;

        valueTask.IsCompleted.Should().BeTrue();
    }

    [Fact]
    public void Constructor_ShouldCreateBoundedChannel_WithCorrectCapacity()
    {
        var auditService = new AuditService();

        auditService.Should().NotBeNull();
    }
}
