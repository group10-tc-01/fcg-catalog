namespace FCG.Catalog.Infrastructure.Kafka.Messages
{
    public class PaymentProcessedMessage
    {
        public Guid CorrelationId { get; set; }
        public Guid PaymentId { get; set; }
        public Guid UserId { get; set; }
        public Guid GameId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public DateTime ProcessedAt { get; set; }
    }
}