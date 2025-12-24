using Confluent.Kafka;
using FCG.Catalog.Infrastructure.Kafka.Abstractions;
using FCG.Catalog.Infrastructure.Kafka.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;

namespace FCG.Catalog.Infrastructure.Kafka.Services
{
    [ExcludeFromCodeCoverage]
    public class KafkaConsumerService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly KafkaSettings _settings;
        private readonly ILogger<KafkaConsumerService> _logger;
        private readonly List<Task> _consumerTasks = [];

        public KafkaConsumerService(IServiceProvider serviceProvider, IOptions<KafkaSettings> options, ILogger<KafkaConsumerService> logger)
        {
            _serviceProvider = serviceProvider;
            _settings = options.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var enabledTopics = _settings.Topics.ConsumerTopics
                .Where(t => t.Enabled)
                .ToList();

            if (!enabledTopics.Any())
            {
                _logger.LogWarning("Nenhum tópico de consumer habilitado");
                return;
            }

            _logger.LogInformation(
                "Iniciando consumers para {Count} tópicos: {Topics}",
                enabledTopics.Count,
                string.Join(", ", enabledTopics.Select(t => t.TopicName)));

            foreach (var topicConfig in enabledTopics)
            {
                var task = Task.Run(
                    () => ConsumeTopicAsync(topicConfig, stoppingToken),
                    stoppingToken);

                _consumerTasks.Add(task);
            }

            await Task.WhenAll(_consumerTasks);
        }

        private async Task ConsumeTopicAsync(ConsumerTopicConfiguration topicConfig, CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();

            var handler = scope.ServiceProvider
                .GetServices<IKafkaConsumer>()
                .FirstOrDefault(h => h.Topic == topicConfig.TopicName);

            if (handler == null)
            {
                _logger.LogWarning(
                    "Handler não encontrado para o tópico {Topic}. " +
                    "Certifique-se de que o handler está registrado no DI.",
                    topicConfig.TopicName);
                return;
            }

            var config = new ConsumerConfig
            {
                BootstrapServers = _settings.BootstrapServers,
                GroupId = _settings.Consumer.GroupId,
                ClientId = $"{_settings.ClientId}-consumer-{topicConfig.TopicName}",
                EnableAutoCommit = _settings.Consumer.EnableAutoCommit,
                SessionTimeoutMs = _settings.Consumer.SessionTimeoutMs,
                MaxPollIntervalMs = _settings.Consumer.MaxPollIntervalMs,
                AutoOffsetReset = Enum.Parse<AutoOffsetReset>(
                    _settings.Consumer.AutoOffsetReset, true)
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config)
                .SetErrorHandler((_, e) =>
                {
                    _logger.LogError(
                        "Erro no consumer do tópico {Topic}: {Reason}",
                        topicConfig.TopicName, e.Reason);
                })
                .Build();

            consumer.Subscribe(topicConfig.TopicName);

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var consumeResult = consumer.Consume(cancellationToken);

                        if (consumeResult?.Message?.Value != null)
                        {
                            _logger.LogDebug(
                                "Mensagem recebida do tópico {Topic} - Partition: {Partition}, Offset: {Offset}",
                                topicConfig.TopicName,
                                consumeResult.Partition.Value,
                                consumeResult.Offset.Value);

                            await handler.HandleAsync(
                                consumeResult.Message.Value,
                                cancellationToken);

                            if (_settings.Consumer.EnableAutoCommit) continue;
                            consumer.Commit(consumeResult);

                            _logger.LogDebug("Commit realizado para tópico {Topic} - Offset: {Offset}", topicConfig.TopicName, consumeResult.Offset.Value);
                        }
                    }
                    catch (ConsumeException ex)
                    {
                        _logger.LogError(
                            ex,
                            "Erro ao consumir mensagem do tópico {Topic}",
                            topicConfig.TopicName);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            ex,
                            "Erro inesperado ao processar mensagem do tópico {Topic}",
                            topicConfig.TopicName);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation(
                    "Consumer do tópico {Topic} foi cancelado",
                    topicConfig.TopicName);
            }
            finally
            {
                try
                {
                    consumer.Close();
                    _logger.LogInformation(
                        "Consumer do tópico {Topic} foi fechado",
                        topicConfig.TopicName);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Erro ao fechar consumer do tópico {Topic}",
                        topicConfig.TopicName);
                }
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Kafka Consumer Service parando...");

            try
            {
                await Task.WhenAll(_consumerTasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao parar consumers");
            }

            await base.StopAsync(cancellationToken);

            _logger.LogInformation("Kafka Consumer Service parado com sucesso");
        }
    }
}