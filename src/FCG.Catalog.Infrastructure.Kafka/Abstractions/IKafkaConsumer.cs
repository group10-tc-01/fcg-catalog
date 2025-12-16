namespace FCG.Catalog.Infrastructure.Kafka.Abstractions
{
    public interface IKafkaConsumer
    {
        string Topic { get; }
        Task HandleAsync(string message, CancellationToken cancellationToken);
    }
}
