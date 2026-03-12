using Bogus;
using FCG.Catalog.Domain.Catalog.Entities.Audit;

namespace FCG.Catalog.CommomTestUtilities.Builders.Audit;

public class AuditEntryBuilder
{
    private readonly Guid _id = Guid.NewGuid();
    private string _entityName = "Game";
    private string _action = "Create";
    private string _entityPrimaryKey = Guid.NewGuid().ToString();
    private string? _oldValue;
    private string? _newValue;
    private DateTime _occurredAt = DateTime.UtcNow;
    private Guid _userId = Guid.NewGuid();
    private string? _userName = "testuser@example.com";
    private Guid? _correlationId = Guid.NewGuid();

    public AuditEntry Build()
    {
        return new AuditEntry(
            _entityName,
            _action,
            _entityPrimaryKey,
            _oldValue,
            _newValue,
            _occurredAt,
            _userId,
            _userName,
            _correlationId);
    }

    public AuditEntryBuilder WithEntityName(string entityName)
    {
        _entityName = entityName;
        return this;
    }

    public AuditEntryBuilder WithAction(string action)
    {
        _action = action;
        return this;
    }

    public AuditEntryBuilder WithEntityPrimaryKey(string primaryKey)
    {
        _entityPrimaryKey = primaryKey;
        return this;
    }

    public AuditEntryBuilder WithOldValue(string? oldValue)
    {
        _oldValue = oldValue;
        return this;
    }

    public AuditEntryBuilder WithNewValue(string? newValue)
    {
        _newValue = newValue;
        return this;
    }

    public AuditEntryBuilder WithUserId(Guid userId)
    {
        _userId = userId;
        return this;
    }

    public AuditEntryBuilder WithUserName(string? userName)
    {
        _userName = userName;
        return this;
    }

    public AuditEntryBuilder WithCorrelationId(Guid? correlationId)
    {
        _correlationId = correlationId;
        return this;
    }

    public AuditEntryBuilder WithOccurredAt(DateTime occurredAt)
    {
        _occurredAt = occurredAt;
        return this;
    }

    public static List<AuditEntry> BuildList(int count)
    {
        var faker = new Faker();
        return Enumerable.Range(0, count)
            .Select(_ => new AuditEntryBuilder().Build())
            .ToList();
    }

    public static AuditEntry CreateForCreate(string entityName, string entityPrimaryKey, string? newValue = null)
    {
        return new AuditEntryBuilder()
            .WithEntityName(entityName)
            .WithAction("Create")
            .WithEntityPrimaryKey(entityPrimaryKey)
            .WithOldValue(null)
            .WithNewValue(newValue)
            .Build();
    }

    public static AuditEntry CreateForUpdate(string entityName, string entityPrimaryKey, string oldValue, string newValue)
    {
        return new AuditEntryBuilder()
            .WithEntityName(entityName)
            .WithAction("Update")
            .WithEntityPrimaryKey(entityPrimaryKey)
            .WithOldValue(oldValue)
            .WithNewValue(newValue)
            .Build();
    }

    public static AuditEntry CreateForDelete(string entityName, string entityPrimaryKey, string oldValue)
    {
        return new AuditEntryBuilder()
            .WithEntityName(entityName)
            .WithAction("Delete")
            .WithEntityPrimaryKey(entityPrimaryKey)
            .WithOldValue(oldValue)
            .WithNewValue(null)
            .Build();
    }
}
