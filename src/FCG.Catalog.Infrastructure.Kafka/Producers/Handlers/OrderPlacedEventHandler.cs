using FCG.Catalog.Domain.Catalog.Events;
using FCG.Catalog.Infrastructure.Kafka.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FCG.Catalog.Infrastructure.Kafka.Producers.Handlers
{
    public class OrderPlacedEventHandler : INotificationHandler<OrderPlacedEvent>
    {
        private readonly KafkaProducerService _kafkaProducerService;
        private readonly ILogger<OrderPlacedEventHandler> _logger;
        private const string TOPIC_KEY = "order-placed";

        public OrderPlacedEventHandler(KafkaProducerService kafkaProducerService, ILogger<OrderPlacedEventHandler> logger)
        {
            _kafkaProducerService = kafkaProducerService;
            _logger = logger;
        }

        public async Task Handle(OrderPlacedEvent notification, CancellationToken cancellationToken)
        {
            await _kafkaProducerService.PublishAsync(TOPIC_KEY, notification, cancellationToken);
        }
    }
}
