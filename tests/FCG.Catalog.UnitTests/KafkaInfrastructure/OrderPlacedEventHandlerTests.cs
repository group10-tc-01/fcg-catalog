using FCG.Catalog.Domain.Catalog.Events;
using FCG.Catalog.Infrastructure.Kafka.Producers.Handlers;
using FCG.Catalog.Infrastructure.Kafka.Services.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace FCG.Catalog.UnitTests.KafkaInfrastructure
{
    public class OrderPlacedEventHandlerTests
    {
        private readonly Mock<IKafkaProducerService> _kafkaProducerServiceMock;
        private readonly Mock<ILogger<OrderPlacedEventHandler>> _loggerMock;
        private readonly OrderPlacedEventHandler _handler;

        public OrderPlacedEventHandlerTests()
        {
            _kafkaProducerServiceMock = new Mock<IKafkaProducerService>();
            _loggerMock = new Mock<ILogger<OrderPlacedEventHandler>>();
            _handler = new OrderPlacedEventHandler(_kafkaProducerServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldPublishEventToKafka_WhenCalled()
        {
            // Arrange
            var orderEvent = new OrderPlacedEvent(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                59.99m,
                DateTimeOffset.UtcNow
            );

            _kafkaProducerServiceMock
                .Setup(x => x.PublishAsync("order-placed", orderEvent, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(orderEvent, CancellationToken.None);

            // Assert
            _kafkaProducerServiceMock.Verify(
                x => x.PublishAsync("order-placed", orderEvent, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldUseCorrectTopicKey_WhenPublishing()
        {
            // Arrange
            var orderEvent = new OrderPlacedEvent(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                100m,
                DateTimeOffset.UtcNow
            );

            _kafkaProducerServiceMock
                .Setup(x => x.PublishAsync(It.IsAny<string>(), It.IsAny<OrderPlacedEvent>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(orderEvent, CancellationToken.None);

            // Assert
            _kafkaProducerServiceMock.Verify(
                x => x.PublishAsync("order-placed", It.IsAny<OrderPlacedEvent>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldPassCancellationToken_WhenPublishing()
        {
            // Arrange
            var orderEvent = new OrderPlacedEvent(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                75.50m,
                DateTimeOffset.UtcNow
            );

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;

            _kafkaProducerServiceMock
                .Setup(x => x.PublishAsync("order-placed", orderEvent, cancellationToken))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(orderEvent, cancellationToken);

            // Assert
            _kafkaProducerServiceMock.Verify(
                x => x.PublishAsync("order-placed", orderEvent, cancellationToken),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldRespectCancellationToken_WhenCancelled()
        {
            // Arrange
            var orderEvent = new OrderPlacedEvent(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                50m,
                DateTimeOffset.UtcNow
            );

            var cts = new CancellationTokenSource();
            cts.Cancel();

            _kafkaProducerServiceMock
                .Setup(x => x.PublishAsync("order-placed", orderEvent, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new OperationCanceledException());

            // Act
            Func<Task> act = async () => await _handler.Handle(orderEvent, cts.Token);

            // Assert
            await act.Should().ThrowAsync<OperationCanceledException>();
        }

        [Fact]
        public async Task Handle_ShouldPropagateException_WhenKafkaServiceFails()
        {
            // Arrange
            var orderEvent = new OrderPlacedEvent(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                99.99m,
                DateTimeOffset.UtcNow
            );

            var expectedException = new InvalidOperationException("Kafka connection failed");

            _kafkaProducerServiceMock
                .Setup(x => x.PublishAsync("order-placed", orderEvent, It.IsAny<CancellationToken>()))
                .ThrowsAsync(expectedException);

            // Act
            Func<Task> act = async () => await _handler.Handle(orderEvent, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Kafka connection failed");
        }

        [Fact]
        public async Task Handle_ShouldPublishEventWithAllProperties_WhenCalled()
        {
            // Arrange
            var correlationId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var gameId = Guid.NewGuid();
            var amount = 149.99m;
            var occurredOn = DateTimeOffset.UtcNow;

            var orderEvent = new OrderPlacedEvent(correlationId, userId, gameId, amount, occurredOn);

            OrderPlacedEvent? capturedEvent = null;

            _kafkaProducerServiceMock
                .Setup(x => x.PublishAsync("order-placed", It.IsAny<OrderPlacedEvent>(), It.IsAny<CancellationToken>()))
                .Callback<string, OrderPlacedEvent, CancellationToken>((_, e, _) => capturedEvent = e)
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(orderEvent, CancellationToken.None);

            // Assert
            capturedEvent.Should().NotBeNull();
            capturedEvent!.CorrelationId.Should().Be(correlationId);
            capturedEvent.UserId.Should().Be(userId);
            capturedEvent.GameId.Should().Be(gameId);
            capturedEvent.Amount.Should().Be(amount);
            capturedEvent.OccurredOn.Should().BeCloseTo(occurredOn, TimeSpan.FromSeconds(1));
        }

        [Theory]
        [InlineData(0.01)]
        [InlineData(9.99)]
        [InlineData(59.99)]
        [InlineData(299.99)]
        public async Task Handle_ShouldPublishEventWithDifferentAmounts_WhenCalled(decimal amount)
        {
            // Arrange
            var orderEvent = new OrderPlacedEvent(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                amount,
                DateTimeOffset.UtcNow
            );

            _kafkaProducerServiceMock
                .Setup(x => x.PublishAsync("order-placed", orderEvent, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(orderEvent, CancellationToken.None);

            // Assert
            _kafkaProducerServiceMock.Verify(
                x => x.PublishAsync("order-placed", It.Is<OrderPlacedEvent>(e => e.Amount == amount), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldCallPublishAsyncOnlyOnce_WhenCalled()
        {
            // Arrange
            var orderEvent = new OrderPlacedEvent(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                79.99m,
                DateTimeOffset.UtcNow
            );

            _kafkaProducerServiceMock
                .Setup(x => x.PublishAsync("order-placed", orderEvent, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(orderEvent, CancellationToken.None);

            // Assert
            _kafkaProducerServiceMock.Verify(
                x => x.PublishAsync(It.IsAny<string>(), It.IsAny<OrderPlacedEvent>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldNotThrowException_WhenPublishSucceeds()
        {
            // Arrange
            var orderEvent = new OrderPlacedEvent(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                120m,
                DateTimeOffset.UtcNow
            );

            _kafkaProducerServiceMock
                .Setup(x => x.PublishAsync("order-placed", orderEvent, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            Func<Task> act = async () => await _handler.Handle(orderEvent, CancellationToken.None);

            // Assert
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task Handle_ShouldPublishToCorrectTopic_WhenMultipleEventsPublished()
        {
            // Arrange
            var event1 = new OrderPlacedEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 10m, DateTimeOffset.UtcNow);
            var event2 = new OrderPlacedEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 20m, DateTimeOffset.UtcNow);
            var event3 = new OrderPlacedEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 30m, DateTimeOffset.UtcNow);

            _kafkaProducerServiceMock
                .Setup(x => x.PublishAsync("order-placed", It.IsAny<OrderPlacedEvent>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(event1, CancellationToken.None);
            await _handler.Handle(event2, CancellationToken.None);
            await _handler.Handle(event3, CancellationToken.None);

            // Assert
            _kafkaProducerServiceMock.Verify(
                x => x.PublishAsync("order-placed", It.IsAny<OrderPlacedEvent>(), It.IsAny<CancellationToken>()),
                Times.Exactly(3));
        }
    }
}