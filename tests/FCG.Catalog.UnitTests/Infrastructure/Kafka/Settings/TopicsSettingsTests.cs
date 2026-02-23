using FCG.Catalog.Infrastructure.Kafka.Settings;

namespace FCG.Catalog.UnitTests.Infrastructure.Kafka.Settings;

public class TopicsSettingsTests
{
    [Fact]
    public void TopicsSettings_ShouldHaveDefaultValues()
    {
        // Arrange & Act
        var settings = new TopicsSettings();

        // Assert
        Assert.NotNull(settings.ConsumerTopics);
        Assert.Empty(settings.ConsumerTopics);
        Assert.NotNull(settings.ProducerTopics);
        Assert.Empty(settings.ProducerTopics);
    }

    [Fact]
    public void TopicsSettings_ShouldAllowPropertyAssignment()
    {
        // Arrange
        var consumerTopics = new List<ConsumerTopicConfiguration>
        {
            new ConsumerTopicConfiguration { TopicName = "topic1", HandlerType = "Handler1" }
        };
        var producerTopics = new Dictionary<string, string>
        {
            { "topic2", "value2" }
        };
        var settings = new TopicsSettings
        {
            ConsumerTopics = consumerTopics,
            ProducerTopics = producerTopics
        };

        // Act & Assert
        Assert.Equal(consumerTopics, settings.ConsumerTopics);
        Assert.Equal(producerTopics, settings.ProducerTopics);
    }
}