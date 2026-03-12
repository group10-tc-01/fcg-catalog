using FCG.Catalog.CommomTestUtilities.Builders.Audit;
using FCG.Catalog.Domain.Catalog.Entities.Audit;
using FluentAssertions;

namespace FCG.Catalog.UnitTests.Domain.Catalog.Entities.Audit;

public class AuditEntryTests
{
    [Fact]
    public void Constructor_ShouldCreateAuditEntry_WithAllProperties()
    {
        var entityName = "Game";
        var action = "Create";
        var entityPrimaryKey = Guid.NewGuid().ToString();
        var oldValue = "{\"Title\":\"Old\"}";
        var newValue = "{\"Title\":\"New\"}";
        var occurredAt = DateTime.UtcNow;
        var userId = Guid.NewGuid();
        var userName = "testuser@example.com";
        var correlationId = Guid.NewGuid();

        var entry = new AuditEntry(
            entityName,
            action,
            entityPrimaryKey,
            oldValue,
            newValue,
            occurredAt,
            userId,
            userName,
            correlationId);

        entry.EntityName.Should().Be(entityName);
        entry.Action.Should().Be(action);
        entry.EntityPrimaryKey.Should().Be(entityPrimaryKey);
        entry.OldValue.Should().Be(oldValue);
        entry.NewValue.Should().Be(newValue);
        entry.OccurredAt.Should().Be(occurredAt);
        entry.UserId.Should().Be(userId);
        entry.UserName.Should().Be(userName);
        entry.CorrelationId.Should().Be(correlationId);
    }

    [Fact]
    public void Constructor_ShouldCreateAuditEntry_WithNullOptionalProperties()
    {
        var entityName = "Game";
        var action = "Create";
        var entityPrimaryKey = Guid.NewGuid().ToString();
        var occurredAt = DateTime.UtcNow;
        var userId = Guid.NewGuid();

        var entry = new AuditEntry(
            entityName,
            action,
            entityPrimaryKey,
            null,
            "{\"Title\":\"New\"}",
            occurredAt,
            userId,
            null,
            null);

        entry.OldValue.Should().BeNull();
        entry.NewValue.Should().NotBeNull();
        entry.UserName.Should().BeNull();
        entry.CorrelationId.Should().BeNull();
    }

    [Fact]
    public void CreateForCreate_ShouldCreateEntryWithCreateAction()
    {
        var entityName = "Game";
        var entityPrimaryKey = Guid.NewGuid().ToString();
        var newValue = "{\"Title\":\"Test\"}";

        var entry = AuditEntryBuilder.CreateForCreate(entityName, entityPrimaryKey, newValue);

        entry.Action.Should().Be("Create");
        entry.EntityName.Should().Be(entityName);
        entry.EntityPrimaryKey.Should().Be(entityPrimaryKey);
        entry.OldValue.Should().BeNull();
        entry.NewValue.Should().Be(newValue);
    }

    [Fact]
    public void CreateForUpdate_ShouldCreateEntryWithUpdateAction()
    {
        var entityName = "Game";
        var entityPrimaryKey = Guid.NewGuid().ToString();
        var oldValue = "{\"Title\":\"Old\"}";
        var newValue = "{\"Title\":\"New\"}";

        var entry = AuditEntryBuilder.CreateForUpdate(entityName, entityPrimaryKey, oldValue, newValue);

        entry.Action.Should().Be("Update");
        entry.EntityName.Should().Be(entityName);
        entry.EntityPrimaryKey.Should().Be(entityPrimaryKey);
        entry.OldValue.Should().Be(oldValue);
        entry.NewValue.Should().Be(newValue);
    }

    [Fact]
    public void CreateForDelete_ShouldCreateEntryWithDeleteAction()
    {
        var entityName = "Game";
        var entityPrimaryKey = Guid.NewGuid().ToString();
        var oldValue = "{\"Title\":\"Deleted\"}";

        var entry = AuditEntryBuilder.CreateForDelete(entityName, entityPrimaryKey, oldValue);

        entry.Action.Should().Be("Delete");
        entry.EntityName.Should().Be(entityName);
        entry.EntityPrimaryKey.Should().Be(entityPrimaryKey);
        entry.OldValue.Should().Be(oldValue);
        entry.NewValue.Should().BeNull();
    }

    [Fact]
    public void BuildList_ShouldCreateListOfEntries()
    {
        var count = 5;

        var entries = AuditEntryBuilder.BuildList(count);

        entries.Should().HaveCount(count);
        entries.Should().AllSatisfy(entry =>
        {
            entry.EntityName.Should().NotBeNullOrEmpty();
            entry.Action.Should().NotBeNullOrEmpty();
            entry.EntityPrimaryKey.Should().NotBeNullOrEmpty();
        });
    }
}
