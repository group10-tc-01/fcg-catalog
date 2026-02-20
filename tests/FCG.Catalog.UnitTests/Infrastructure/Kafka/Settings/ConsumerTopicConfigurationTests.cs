using FCG.Catalog.Infrastructure.Kafka.Settings;

namespace FCG.Catalog.UnitTests.Infrastructure.Kafka.Settings;

public class ConsumerTopicConfigurationTests
{
    [Fact]
    public void ConsumerTopicConfiguration_ShouldHaveDefaultValues()
    {
        // Arrange & Act
        var config = new ConsumerTopicConfiguration();

        // Assert
        Assert.Equal(string.Empty, config.TopicName);
        Assert.Equal(string.Empty, config.HandlerType);
        Assert.Equal(3, config.MaxTries);
        Assert.True(config.Enabled);
    }

    [Fact]
    public void ConsumerTopicConfiguration_ShouldAllowPropertyAssignment()
    {
        // Arrange
        var config = new ConsumerTopicConfiguration
        {
            TopicName = "test-topic",
            HandlerType = "TestHandler",
            MaxTries = 5,
            Enabled = false
        };

        // Act & Assert
        Assert.Equal("test-topic", config.TopicName);
        Assert.Equal("TestHandler", config.HandlerType);
        Assert.Equal(5, config.MaxTries);
        Assert.False(config.Enabled);
    }
}