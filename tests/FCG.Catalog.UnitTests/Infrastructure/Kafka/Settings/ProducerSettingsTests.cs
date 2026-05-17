using Confluent.Kafka;
using FCG.Catalog.Infrastructure.Kafka.Settings;

namespace FCG.Catalog.UnitTests.Infrastructure.Kafka.Settings;

public class ProducerSettingsTests
{
    [Fact]
    public void ProducerSettings_ShouldHaveDefaultValues()
    {
        // Arrange & Act
        var settings = new ProducerSettings();

        // Assert
        Assert.False(settings.EnableIdempotence);
        Assert.Equal(Acks.Leader, settings.Acks);
        Assert.Equal(1, settings.MaxInFlight);
        Assert.Equal(3, settings.Retries);
        Assert.Equal(100, settings.RetryBackoffMs);
        Assert.Equal(CompressionType.None, settings.CompressionType);
    }

    [Fact]
    public void ProducerSettings_ShouldAllowPropertyAssignment()
    {
        // Arrange
        var settings = new ProducerSettings
        {
            EnableIdempotence = false,
            Acks = Acks.Leader,
            MaxInFlight = 10,
            Retries = 5,
            RetryBackoffMs = 200,
            CompressionType = CompressionType.Gzip
        };

        // Act & Assert
        Assert.False(settings.EnableIdempotence);
        Assert.Equal(Acks.Leader, settings.Acks);
        Assert.Equal(10, settings.MaxInFlight);
        Assert.Equal(5, settings.Retries);
        Assert.Equal(200, settings.RetryBackoffMs);
        Assert.Equal(CompressionType.Gzip, settings.CompressionType);
    }
}
