namespace FCG.Catalog.Infrastructure.Redis.Settings
{
    public sealed class RedisSettings
    {
        public const string SectionName = "RedisSettings";

        public string ConnectionString { get; set; } = string.Empty;
        public string InstanceName { get; set; } = "fcg-catalog:";
        public int DefaultTtlSeconds { get; set; } = 60;
    }
}
