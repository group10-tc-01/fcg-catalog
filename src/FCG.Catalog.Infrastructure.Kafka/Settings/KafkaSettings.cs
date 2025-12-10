namespace FCG.Catalog.Infrastructure.Kafka.Settings
{
    public class KafkaSettings
    {
        public string BootstrapServers { get; set; }
        public string GroupId { get; set; }
        public bool EnableAutoCommit { get; set; } = false;
        public int SessionTimeoutMs { get; set; } = 10000;
        public string AutoOffsetReset { get; set; } = "earliest";
        public List<TopicConfiguration> Topics { get; set; } = new();
    }

    public class TopicConfiguration
    {
        public string TopicName { get; set; } = string.Empty;
        public string HandlerType { get; set; } = string.Empty;
        public int MaxTries { get; set; } = 3;
        public bool Enabled { get; set; } = true;
    }
}
