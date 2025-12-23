using FCG.Catalog.Infrastructure.Kafka.Abstractions;
using FCG.Catalog.Infrastructure.Kafka.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FCG.Catalog.Infrastructure.Kafka.Services
{
    public class KafkaProducerService
    {
        private readonly IKafkaProducer _producer;
        private readonly KafkaSettings _settings;
        private readonly ILogger<KafkaProducerService> _logger;

        public KafkaProducerService(IKafkaProducer producer, IOptions<KafkaSettings> options, ILogger<KafkaProducerService> logger)
        {
            _producer = producer;
            _settings = options.Value;
            _logger = logger;
        }

        public async Task PublishAsync<T>(string topicKey, T message, CancellationToken cancellationToken = default)
        {
            if (!_settings.Topics.ProducerTopics.TryGetValue(topicKey, out var topicName))
            {
                throw new ArgumentException($"Tópico com chave '{topicKey}' não encontrado", nameof(topicKey));
            }

            _logger.LogDebug("Publicando mensagem no tópico {TopicName} (chave: {TopicKey})", topicName, topicKey);

            await _producer.ProduceAsync(topicName, message, cancellationToken);
        }
    }
}
