
using FCG.Catalog.Application.UseCases.Games.ProcessPaymentResult;
using FCG.Catalog.Infrastructure.Kafka.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FCG.Catalog.Infrastructure.Kafka.Consumers
{
    public class PaymentProcessedEventHandler : KafkaConsumerBase<PaymentProcessedMessage, ProcessPaymentResultInput>
    {
        public PaymentProcessedEventHandler(
            IServiceScopeFactory serviceScopeFactory,
            ILogger<PaymentProcessedEventHandler> logger)
            : base(serviceScopeFactory, logger)
        {
        }

        public override string Topic => "payment-processed";

        protected override ProcessPaymentResultInput MapToCommand(PaymentProcessedMessage message)
        {
            bool isApproved = string.Equals(message.Status, "Approved", StringComparison.OrdinalIgnoreCase);

            return new ProcessPaymentResultInput
            {
                CorrelationId = message.CorrelationId,
                UserId = message.UserId,
                GameId = message.GameId,
                Amount = message.Amount,
                IsApproved = isApproved
            };
        }
    }
}