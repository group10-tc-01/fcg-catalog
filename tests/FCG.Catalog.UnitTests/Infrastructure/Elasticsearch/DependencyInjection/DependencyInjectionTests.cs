using Elastic.Clients.Elasticsearch;
using FCG.Catalog.Infrastructure.Elasticsearch.DependencyInjection;
using FCG.Catalog.Infrastructure.Elasticsearch.Settings;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FCG.Catalog.UnitTests.Infrastructure.Elasticsearch.DependencyInjection
{
    public class DependencyInjectionTests
    {
        [Fact]
        public void AddElasticsearchInfrastructure_ShouldRegisterSettingsAndClient()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    [$"{ElasticsearchSettings.SectionName}:Uri"] = "http://localhost:9200",
                    [$"{ElasticsearchSettings.SectionName}:IndexName"] = "games",
                    [$"{ElasticsearchSettings.SectionName}:Username"] = string.Empty,
                    [$"{ElasticsearchSettings.SectionName}:Password"] = string.Empty
                })
                .Build();

            var services = new ServiceCollection();

            services.AddElasticsearchInfrastructure(configuration);

            var provider = services.BuildServiceProvider();

            provider.GetRequiredService<IOptions<ElasticsearchSettings>>()
                .Value
                .Should()
                .BeEquivalentTo(new ElasticsearchSettings
                {
                    Uri = "http://localhost:9200",
                    IndexName = "games",
                    Username = string.Empty,
                    Password = string.Empty
                });

            provider.GetRequiredService<ElasticsearchClient>().Should().NotBeNull();
        }
    }
}
