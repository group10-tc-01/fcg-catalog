using FCG.Catalog.Infrastructure.Kafka.Settings;

namespace FCG.Catalog.UnitTests.Infrastructure.Kafka.Settings;

public class ConsumerSettingsTests
{
    [Fact]
    public void ConsumerSettings_ShouldHaveDefaultValues()
    {
        // Arrange & Act
        var settings = new ConsumerSettings();

        // Assert
        Assert.Equal(string.Empty, settings.GroupId);
        Assert.False(settings.EnableAutoCommit);
        Assert.Equal(10000, settings.SessionTimeoutMs);
        Assert.Equal("earliest", settings.AutoOffsetReset);
        Assert.Equal(300000, settings.MaxPollIntervalMs);
    }

    [Fact]
    public void ConsumerSettings_ShouldAllowPropertyAssignment()
    {
        // Arrange
        var settings = new ConsumerSettings
        {
            GroupId = "test-group",
            EnableAutoCommit = true,
            SessionTimeoutMs = 15000,
            AutoOffsetReset = "latest",
            MaxPollIntervalMs = 400000
        };

        // Act & Assert
        Assert.Equal("test-group", settings.GroupId);
        Assert.True(settings.EnableAutoCommit);
        Assert.Equal(15000, settings.SessionTimeoutMs);
        Assert.Equal("latest", settings.AutoOffsetReset);
        Assert.Equal(400000, settings.MaxPollIntervalMs);
    }
}