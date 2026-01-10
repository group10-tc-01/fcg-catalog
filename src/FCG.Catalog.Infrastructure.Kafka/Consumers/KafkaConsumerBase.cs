using FCG.Catalog.Infrastructure.Kafka.Abstractions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace FCG.Catalog.Infrastructure.Kafka.Consumers
{
    [ExcludeFromCodeCoverage]
    public abstract class KafkaConsumerBase<TMessage, TCommand> : IKafkaConsumer where TMessage : class where TCommand : IRequest
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        protected readonly ILogger _logger;
        private readonly IAsyncPolicy _retryPolicy;
        public abstract string Topic { get; }

        protected KafkaConsumerBase(IServiceScopeFactory serviceScopeFactory, ILogger logger, int maxTries = 3)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;

            _retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    maxTries,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning(
                            exception,
                            "Retry {Retry} após {Delay}s para tópico {Topic}",
                            retryCount, timeSpan.TotalSeconds, Topic);
                    });
        }

        public async Task HandleAsync(string message, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Processando mensagem do tópico {Topic}: {Message}", Topic, message);

                await _retryPolicy.ExecuteAsync(async () =>
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true 
                    };

                    var kafkaMessage = JsonSerializer.Deserialize<TMessage>(message, options);

                    if (kafkaMessage == null)
                        throw new InvalidOperationException("Mensagem invalida ou nula.");

                    var command = MapToCommand(kafkaMessage);
                    
                    using var scope = _serviceScopeFactory.CreateScope();
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    await mediator.Send(command, cancellationToken);
                    
                    _logger.LogInformation(
                        "Mensagem processada com sucesso no tópico {Topic}", 
                        Topic);

                });
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar mensagem do tópico {Topic}: {Message}", Topic, message);
                throw;
            }
        }

        protected abstract TCommand MapToCommand(TMessage message);
    }
}
