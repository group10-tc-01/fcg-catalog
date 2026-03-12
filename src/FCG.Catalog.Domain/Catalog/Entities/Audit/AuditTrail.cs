using FCG.Catalog.Domain.Abstractions;

namespace FCG.Catalog.Domain.Catalog.Entities.Audit;

public sealed class AuditTrail
{
    public Guid Id { get; private set; }
    public string EntityName { get; private set;  } 
    public string Action { get; private set;  }
    public string? OldValue { get; private set; }
    public string? NewValue { get; private set;  }
    public DateTime OccurredAt { get; private set;  }
    public Guid UserId { get; private set;  }
    public string? UserName { get; private set;  }
    public string EntityPrimaryKey { get; private set;  }
    public Guid? CorrelationId { get; private set; }
    
    
    private AuditTrail() { }

    public AuditTrail(string entityName, string action, string entityPrimaryKey, string? oldValue, string? newValue, Guid userId, string? userName, Guid? correlationId, string? ipAddress)
    {
        Id = Guid.NewGuid();
        EntityName = entityName;
        Action = action;
        EntityPrimaryKey = entityPrimaryKey;
        OldValue = oldValue;
        NewValue = newValue;
        OccurredAt = DateTime.UtcNow; 
        UserId = userId;
        UserName = userName;
        CorrelationId = correlationId;
    }
}