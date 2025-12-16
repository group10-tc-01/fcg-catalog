using Confluent.Kafka;

namespace FCG.Catalog.Infrastructure.Kafka.Abstractions
{ 
    public interface IKafkaProducer 
    {
        Task<DeliveryResult<string, string>> ProduceAsync<T>(string topic, T message, CancellationToken cancellationToken);
    }
}
