using Confluent.Kafka;
using System.Diagnostics.CodeAnalysis;

namespace FCG.Catalog.Infrastructure.Kafka.Settings
{
    [ExcludeFromCodeCoverage]
    public class KafkaSettings
    {
        public string BootstrapServers { get; set; } = string.Empty;
        public string ClientId { get; set; } = "fcg-catalog";
        public ConsumerSettings Consumer { get; set; } = new();
        public ProducerSettings Producer { get; set; } = new();
        public TopicsSettings Topics { get; set; } = new();
    }

    public class ConsumerSettings
    {
        public string GroupId { get; set; } = string.Empty;
        public bool EnableAutoCommit { get; set; } = false;
        public int SessionTimeoutMs { get; set; } = 10000;
        public string AutoOffsetReset { get; set; } = "earliest";
        public int MaxPollIntervalMs { get; set; } = 300000;
    }

    public class ProducerSettings
    {
        public bool EnableIdempotence { get; set; } = true;
        public Acks Acks { get; set; } = Acks.All;
        public int MaxInFlight { get; set; } = 5;
        public int Retries { get; set; } = 3;
        public int RetryBackoffMs { get; set; } = 100;
        public CompressionType CompressionType { get; set; } = CompressionType.Snappy;
    }

    public class TopicsSettings
    {
        public List<ConsumerTopicConfiguration> ConsumerTopics { get; set; } = [];

        public Dictionary<string, string> ProducerTopics { get; set; } = new()
        {
            { "UserCreated", "user-created" }
        };
    }

    public class ConsumerTopicConfiguration
    {
        public string TopicName { get; set; } = string.Empty;
        public string HandlerType { get; set; } = string.Empty;
        public int MaxTries { get; set; } = 3;
        public bool Enabled { get; set; } = true;
    }
}