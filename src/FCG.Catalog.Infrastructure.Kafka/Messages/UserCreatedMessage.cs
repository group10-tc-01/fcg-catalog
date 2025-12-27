namespace FCG.Catalog.Infrastructure.Kafka.Messages
{
    public class UserCreatedMessage
    {
        public Guid UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Guid CorrelationId { get; set; }
        public DateTime OccurredAt { get; set; }
    }
}
