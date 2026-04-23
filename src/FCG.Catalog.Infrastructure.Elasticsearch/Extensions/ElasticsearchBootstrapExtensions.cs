using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.IndexManagement;
using Elastic.Clients.Elasticsearch.Mapping;
using FCG.Catalog.Infrastructure.Elasticsearch.Documents;
using FCG.Catalog.Infrastructure.Elasticsearch.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;

namespace FCG.Catalog.Infrastructure.Elasticsearch.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class ElasticsearchBootstrapExtensions
    {
        public static async Task EnsureGamesIndexCreatedAsync(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();

            var client = scope.ServiceProvider.GetRequiredService<ElasticsearchClient>();
            var settings = scope.ServiceProvider.GetRequiredService<IOptions<ElasticsearchSettings>>().Value;
            var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
                .CreateLogger("FCG.Catalog.Infrastructure.Elasticsearch");

            var existsResponse = await client.Indices.ExistsAsync(settings.IndexName);

            if (existsResponse.Exists)
            {
                logger.LogInformation("Elasticsearch index {IndexName} already exists", settings.IndexName);
                return;
            }

            logger.LogInformation("Creating Elasticsearch index {IndexName}", settings.IndexName);

            var createRequest = new CreateIndexRequest(settings.IndexName)
            {
                Settings = new IndexSettings
                {
                    NumberOfShards = "1",
                    NumberOfReplicas = "0"
                },
                Mappings = new TypeMapping
                {
                    Properties = new Properties
                    {
                        { "id", new KeywordProperty() },
                        {
                            "title",
                            new TextProperty
                            {
                                Fields = new Properties
                                {
                                    { "keyword", new KeywordProperty() }
                                }
                            }
                        },
                        { "description", new TextProperty() },
                        { "category", new KeywordProperty() },
                        { "price", new ScaledFloatNumberProperty { ScalingFactor = 100 } },
                        { "isActive", new BooleanProperty() },
                        { "createdAt", new DateProperty() },
                        { "updatedAt", new DateProperty() }
                    }
                }
            };

            var createResponse = await client.Indices.CreateAsync(createRequest);

            if (!createResponse.IsValidResponse)
            {
                throw new InvalidOperationException(
                    $"Failed to create Elasticsearch index '{settings.IndexName}'.");
            }

            logger.LogInformation("Elasticsearch index {IndexName} created successfully", settings.IndexName);
        }
    }
}
