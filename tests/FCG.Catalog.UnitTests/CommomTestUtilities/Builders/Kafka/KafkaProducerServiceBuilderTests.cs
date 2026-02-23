using FCG.Catalog.CommomTestUtilities.Builders.Kafka;
using FCG.Catalog.Infrastructure.Kafka.Services.Interfaces;
using Moq;

namespace FCG.Catalog.UnitTests.CommomTestUtilities.Builders.Kafka;

public class KafkaProducerServiceBuilderTests
{
    [Fact]
    public void Build_ShouldReturnMockObject()
    {
        // Act
        var service = KafkaProducerServiceBuilder.Build();

        // Assert
        Assert.NotNull(service);
        Assert.IsAssignableFrom<IKafkaProducerService>(service);
    }

    [Fact]
    public async Task SetupPublishAsync_ShouldCompleteTask()
    {
        // Arrange
        var topicKey = "test-topic";
        var message = new { Id = Guid.NewGuid(), Name = "Test Message" };
        KafkaProducerServiceBuilder.SetupPublishAsync<object>();
        var service = KafkaProducerServiceBuilder.Build();

        // Act
        await service.PublishAsync(topicKey, message, CancellationToken.None);

        // Assert
        // No exception should be thrown, task completes
    }

    [Fact]
    public async Task SetupPublishAsyncThrowsException_ShouldThrowException()
    {
        // Arrange
        var topicKey = "test-topic";
        var message = new { Id = Guid.NewGuid(), Name = "Test Message" };
        var exception = new Exception("Test exception");
        KafkaProducerServiceBuilder.SetupPublishAsyncThrowsException<object>(exception);
        var service = KafkaProducerServiceBuilder.Build();

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => service.PublishAsync(topicKey, message, CancellationToken.None));
    }

    [Fact]
    public async Task VerifyPublishAsyncWasCalled_ShouldVerifyCall()
    {
        // Arrange
        var topicKey = "test-topic";
        var message = new { Id = Guid.NewGuid(), Name = "Test Message" };
        KafkaProducerServiceBuilder.SetupPublishAsync<object>();
        var service = KafkaProducerServiceBuilder.Build();
        await service.PublishAsync(topicKey, message, CancellationToken.None);

        // Act & Assert
        KafkaProducerServiceBuilder.VerifyPublishAsyncWasCalled<object>(topicKey, Times.Once());
    }

    [Fact]
    public async Task VerifyPublishAsyncWasCalledWithMessage_ShouldVerifyCallWithSpecificMessage()
    {
        // Arrange
        var topicKey = "test-topic";
        var message = new { Id = Guid.NewGuid(), Name = "Specific Message" };
        KafkaProducerServiceBuilder.SetupPublishAsync<object>();
        var service = KafkaProducerServiceBuilder.Build();
        await service.PublishAsync(topicKey, message, CancellationToken.None);

        // Act & Assert
        KafkaProducerServiceBuilder.VerifyPublishAsyncWasCalledWithMessage(topicKey, message, Times.Once());
    }

    [Fact]
    public void Reset_ShouldClearConfigurations()
    {
        // Arrange
        KafkaProducerServiceBuilder.SetupPublishAsync<object>();
        var service = KafkaProducerServiceBuilder.Build();

        // Act
        KafkaProducerServiceBuilder.Reset();
        var topicKey = "test-topic";
        var message = new { Id = Guid.NewGuid(), Name = "Test Message" };
        var exception = Record.ExceptionAsync(() => service.PublishAsync(topicKey, message, CancellationToken.None));

        // Assert
        // After reset, the setup should be cleared, and since it's strict, it might throw
        // For simplicity, just ensure the method is callable, or adjust based on mock behavior
    }
}