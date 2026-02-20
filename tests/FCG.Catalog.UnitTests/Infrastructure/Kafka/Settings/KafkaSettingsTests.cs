using FCG.Catalog.Infrastructure.Kafka.Settings;

namespace FCG.Catalog.UnitTests.Infrastructure.Kafka.Settings;

public class KafkaSettingsTests
{
    [Fact]
    public void KafkaSettings_ShouldHaveDefaultValues()
    {
        // Arrange & Act
        var settings = new KafkaSettings();

        // Assert
        Assert.Equal(string.Empty, settings.BootstrapServers);
        Assert.Equal("fcg-catalog", settings.ClientId);
        Assert.NotNull(settings.Consumer);
        Assert.NotNull(settings.Producer);
        Assert.NotNull(settings.Topics);
    }

    [Fact]
    public void KafkaSettings_ShouldAllowPropertyAssignment()
    {
        // Arrange
        var settings = new KafkaSettings
        {
            BootstrapServers = "localhost:9092",
            ClientId = "test-client"
        };

        // Act & Assert
        Assert.Equal("localhost:9092", settings.BootstrapServers);
        Assert.Equal("test-client", settings.ClientId);
    }
}