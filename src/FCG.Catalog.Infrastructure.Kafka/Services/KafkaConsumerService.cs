using Confluent.Kafka;
using FCG.Catalog.Infrastructure.Kafka.Consumers.Abstractions;
using FCG.Catalog.Infrastructure.Kafka.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FCG.Catalog.Infrastructure.Kafka.Services
{
    public class KafkaConsumerService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly KafkaSettings _settings;
        private readonly ILogger<KafkaConsumerService> _logger;
        private readonly List<Task> _consumerTasks = [];

        public KafkaConsumerService(IServiceProvider serviceProvider, IOptions<KafkaSettings> settings, ILogger<KafkaConsumerService> logger)
        {
            _serviceProvider = serviceProvider;
            _settings = settings.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Kafka Consumer Service iniciando...");

            var enabledTopics = _settings.Topics.Where(t => t.Enabled).ToList();

            foreach (var topicConfig in enabledTopics)
            {
                var task = Task.Run(
                    () => ConsumeTopicAsync(topicConfig, stoppingToken),
                    stoppingToken);

                _consumerTasks.Add(task);
            }

            await Task.WhenAll(_consumerTasks);
        }

        private async Task ConsumeTopicAsync(
            TopicConfiguration topicConfig,
            CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();

            var handler = scope.ServiceProvider
                .GetServices<IKafkaConsumer>()
                .FirstOrDefault(h => h.Topic == topicConfig.TopicName);

            if (handler == null)
            {
                _logger.LogWarning(
                    "Handler não encontrado para o tópico {Topic}",
                    topicConfig.TopicName);
                return;
            }

            var config = new ConsumerConfig
            {
                BootstrapServers = _settings.BootstrapServers,
                GroupId = _settings.GroupId,
                EnableAutoCommit = _settings.EnableAutoCommit,
                SessionTimeoutMs = _settings.SessionTimeoutMs,
                AutoOffsetReset = Enum.Parse<AutoOffsetReset>(
                    _settings.AutoOffsetReset, true)
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();

            consumer.Subscribe(topicConfig.TopicName);

            _logger.LogInformation(
                "Consumer inscrito no tópico {Topic}",
                topicConfig.TopicName);

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var consumeResult = consumer.Consume(cancellationToken);

                        if (consumeResult?.Message?.Value != null)
                        {
                            await handler.HandleAsync(
                                consumeResult.Message.Value,
                                cancellationToken);

                            if (!_settings.EnableAutoCommit)
                            {
                                consumer.Commit(consumeResult);
                            }
                        }
                    }
                    catch (ConsumeException ex)
                    {
                        _logger.LogError(
                            ex,
                            "Erro ao consumir mensagem do tópico {Topic}",
                            topicConfig.TopicName);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation(
                    "Consumer do tópico {Topic} cancelado",
                    topicConfig.TopicName);
            }
            finally
            {
                consumer.Close();
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Kafka Consumer Service parando...");
            await base.StopAsync(cancellationToken);
        }
    }
}
