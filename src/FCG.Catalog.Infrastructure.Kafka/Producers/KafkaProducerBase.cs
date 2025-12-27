using Confluent.Kafka;
using FCG.Catalog.Infrastructure.Kafka.Abstractions;
using FCG.Catalog.Infrastructure.Kafka.Settings;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace FCG.Catalog.Infrastructure.Kafka.Producers
{
    [ExcludeFromCodeCoverage]
    public class KafkaProducerBase : IKafkaProducer, IDisposable
    {
        private readonly IProducer<string, string> _producer;
        private readonly ILogger<KafkaProducerBase> _logger;
        private readonly JsonSerializerOptions _jsonOptions;
        private bool _disposed;

        public KafkaProducerBase(IOptions<KafkaSettings> settings, ILogger<KafkaProducerBase> logger)
        {
            _logger = logger;
            var kafkaSettings = settings.Value;

            var config = new ProducerConfig
            {
                BootstrapServers = kafkaSettings.BootstrapServers,
                ClientId = $"{kafkaSettings.ClientId}-producer",
                Acks = kafkaSettings.Producer.Acks,
                EnableIdempotence = kafkaSettings.Producer.EnableIdempotence,
                MaxInFlight = kafkaSettings.Producer.MaxInFlight,
                MessageSendMaxRetries = kafkaSettings.Producer.Retries,
                RetryBackoffMs = kafkaSettings.Producer.RetryBackoffMs,
                CompressionType = kafkaSettings.Producer.CompressionType
            };

            _producer = new ProducerBuilder<string, string>(config)
                .SetErrorHandler((_, e) =>
                {
                    _logger.LogError("Erro no Kafka Producer: {Reason}", e.Reason);
                })
                .SetLogHandler((_, log) =>
                {
                    if (log.Level <= SyslogLevel.Warning)
                    {
                        _logger.LogWarning("Kafka Producer Log: {Message}", log.Message);
                    }
                })
                .Build();

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };
            

        }

        public async Task<DeliveryResult<string, string>> ProduceAsync<T>(string topic, T message, CancellationToken cancellationToken = default)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(KafkaProducerBase));

            try
            {
                var json = JsonSerializer.Serialize(message, _jsonOptions);

                var key = Guid.NewGuid().ToString();

                var kafkaMessage = new Message<string, string>
                {
                    Key = key,
                    Value = json,
                    Timestamp = Timestamp.Default
                };

                _logger.LogDebug("Enviando mensagem para tópico {Topic} com Key {Key}", topic, key);

                var result = await _producer.ProduceAsync(topic, kafkaMessage, cancellationToken);

                _logger.LogInformation("Mensagem enviada com sucesso para tópico {Topic} - Partition: {Partition}, Offset: {Offset}",
                    result.Topic,
                    result.Partition.Value,
                    result.Offset.Value);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao produzir mensagem para o tópico {Topic}", topic);
                throw;
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            try
            {
                _logger.LogInformation("Finalizando Kafka Producer...");

                _producer?.Flush(TimeSpan.FromSeconds(10));
                _producer?.Dispose();

                _logger.LogInformation("Kafka Producer finalizado com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao finalizar Kafka Producer");
            }
            finally
            {
                _disposed = true;
            }
        }
    }
}
