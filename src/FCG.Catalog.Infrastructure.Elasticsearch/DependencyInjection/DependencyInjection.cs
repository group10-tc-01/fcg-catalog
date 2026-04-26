using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using FCG.Catalog.Domain.Repositories.Game;
using FCG.Catalog.Infrastructure.Elasticsearch.Repositories;
using FCG.Catalog.Infrastructure.Elasticsearch.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace FCG.Catalog.Infrastructure.Elasticsearch.DependencyInjection
{
    [ExcludeFromCodeCoverage]
    public static class DependencyInjection
    {
        public static IServiceCollection AddElasticsearchInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var elasticsearchSection = configuration.GetSection(ElasticsearchSettings.SectionName);
            var elasticsearchSettings = elasticsearchSection.Get<ElasticsearchSettings>() ?? new ElasticsearchSettings();

            services.Configure<ElasticsearchSettings>(elasticsearchSection);

            services.AddSingleton(_ =>
            {
                var clientSettings = new ElasticsearchClientSettings(new Uri(elasticsearchSettings.Uri))
                    .DefaultIndex(elasticsearchSettings.IndexName);

                if (!string.IsNullOrWhiteSpace(elasticsearchSettings.Username) &&
                    !string.IsNullOrWhiteSpace(elasticsearchSettings.Password))
                {
                    clientSettings = clientSettings.Authentication(
                        new BasicAuthentication(elasticsearchSettings.Username, elasticsearchSettings.Password));
                }

                return new ElasticsearchClient(clientSettings);
            });
            services.AddSingleton<IGameSearchRepository, GameSearchRepository>();

            return services;
        }
    }
}
