using System.Diagnostics.CodeAnalysis;

namespace FCG.Catalog.Infrastructure.Elasticsearch.Settings
{
    [ExcludeFromCodeCoverage]
    public sealed class ElasticsearchSettings
    {
        public const string SectionName = "ElasticsearchSettings";

        public string Uri { get; init; } = string.Empty;

        public string IndexName { get; init; } = string.Empty;

        public string Username { get; init; } = string.Empty;

        public string Password { get; init; } = string.Empty;
    }
}
