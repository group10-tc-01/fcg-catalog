namespace FCG.Catalog.Infrastructure.Kafka.Consumers.Abstractions
{
    public interface IKafkaConsumer
    {
        string Topic { get; }
        Task HandleAsync(string message, CancellationToken cancellationToken);
    }
}
