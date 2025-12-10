using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;
using Polly;

namespace FCG.Catalog.Infrastructure.Kafka.Consumers.Abstractions
{
    public abstract class KafkaConsumerBase<TMessage, TCommand> : IKafkaConsumer 
        where TMessage : class
        where TCommand : IRequest
    {
        protected readonly IMediator _mediator;
        protected readonly ILogger _logger;
        private readonly IAsyncPolicy _retryPolicy;
        public abstract string Topic { get; }


        protected KafkaConsumerBase(IMediator mediator, ILogger logger, int maxTries = 3)
        {
            _mediator = mediator;
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
                    var kafkaMessage = JsonSerializer.Deserialize<TMessage>(message);

                    if (kafkaMessage == null)
                        throw new InvalidOperationException("Mensagem invalida ou nula.");

                    var command = MapToCommand(kafkaMessage);

                    await _mediator.Send(command, cancellationToken);
                });

                _logger.LogInformation(
                    "Mensagem processada com sucesso do tópico {Topic}",
                    Topic);
            }

            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Erro ao processar mensagem do tópico {Topic}: {Message}",
                    Topic, message);

                throw;
            }
        }

        protected abstract TCommand MapToCommand(TMessage message);
    }
}
