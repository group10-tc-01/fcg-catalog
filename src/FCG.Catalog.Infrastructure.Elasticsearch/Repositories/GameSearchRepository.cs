using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using FCG.Catalog.Domain.Models;
using FCG.Catalog.Domain.Repositories.Game;
using FCG.Catalog.Infrastructure.Elasticsearch.Documents;
using FCG.Catalog.Infrastructure.Elasticsearch.Settings;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using TransportHttpMethod = Elastic.Transport.HttpMethod;

namespace FCG.Catalog.Infrastructure.Elasticsearch.Repositories
{
    [ExcludeFromCodeCoverage]
    public sealed class GameSearchRepository : IGameSearchRepository
    {
        private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);

        private readonly ElasticsearchClient _client;
        private readonly string _indexName;

        public GameSearchRepository(ElasticsearchClient client, IOptions<ElasticsearchSettings> settings)
        {
            _client = client;
            _indexName = settings.Value.IndexName;
        }

        public async Task IndexAsync(GameSearch game, CancellationToken cancellationToken = default)
        {
            var indexedAt = DateTime.UtcNow;
            var document = MapToDocument(game, indexedAt);

            var response = await _client.Transport.RequestAsync<StringResponse>(
                TransportHttpMethod.PUT,
                $"/{_indexName}/_doc/{game.Id}",
                PostData.Serializable(document),
                cancellationToken);

            EnsureSuccess(response, $"Failed to index game search document '{game.Id}'.");
        }

        public async Task<PagedListResponse<GameSearch>> SearchAsync(
            string term,
            PaginationParams pagination,
            CancellationToken cancellationToken = default)
        {
            var from = (pagination.PageNumber - 1) * pagination.PageSize;

            var request = new
            {
                from,
                size = pagination.PageSize,
                track_total_hits = true,
                query = new
                {
                    @bool = new
                    {
                        filter = new object[]
                        {
                            new
                            {
                                term = new
                                {
                                    isActive = new
                                    {
                                        value = true
                                    }
                                }
                            }
                        },
                        must = new object[]
                        {
                            new
                            {
                                multi_match = new
                                {
                                    query = term,
                                    fields = new[]
                                    {
                                        "title^2.0",
                                        "description"
                                    },
                                    fuzziness = "AUTO",
                                    type = "best_fields"
                                }
                            }
                        }
                    }
                },
                sort = new object[]
                {
                    new
                    {
                        _score = new
                        {
                            order = "desc"
                        }
                    }
                }
            };

            var response = await _client.Transport.RequestAsync<StringResponse>(
                TransportHttpMethod.POST,
                $"/{_indexName}/_search",
                PostData.Serializable(request),
                cancellationToken);

            EnsureSuccess(response, "Failed to search games in Elasticsearch.");

            using var json = JsonDocument.Parse(response.Body);
            var hits = json.RootElement.GetProperty("hits");
            var totalCount = hits.GetProperty("total").GetProperty("value").GetInt32();

            var items = hits.GetProperty("hits")
                .EnumerateArray()
                .Select(hit => JsonSerializer.Deserialize<GameSearchDocument>(
                    hit.GetProperty("_source").GetRawText(),
                    JsonSerializerOptions)!)
                .Select(MapToModel)
                .ToList();

            return new PagedListResponse<GameSearch>(items, totalCount, pagination.PageNumber, pagination.PageSize);
        }

        public async Task DeleteAsync(Guid gameId, CancellationToken cancellationToken = default)
        {
            var response = await _client.Transport.RequestAsync<StringResponse>(
                TransportHttpMethod.DELETE,
                $"/{_indexName}/_doc/{gameId}",
                cancellationToken);

            var statusCode = response.ApiCallDetails?.HttpStatusCode;

            if (statusCode == (int)System.Net.HttpStatusCode.NotFound)
            {
                return;
            }

            EnsureSuccess(response, $"Failed to delete game search document '{gameId}'.");
        }

        private static GameSearchDocument MapToDocument(GameSearch game, DateTime indexedAt)
        {
            return new GameSearchDocument
            {
                Id = game.Id.ToString(),
                Title = game.Title,
                Description = game.Description,
                Price = game.Price,
                Category = game.Category,
                DiscountedPrice = game.DiscountedPrice,
                IsActive = game.IsActive,
                IndexedAt = indexedAt
            };
        }

        private static GameSearch MapToModel(GameSearchDocument document)
        {
            return new GameSearch
            {
                Id = Guid.Parse(document.Id),
                Title = document.Title,
                Description = document.Description,
                Price = document.Price,
                Category = document.Category,
                DiscountedPrice = document.DiscountedPrice,
                IsActive = document.IsActive,
                IndexedAt = document.IndexedAt
            };
        }

        private static void EnsureSuccess(TransportResponse response, string message)
        {
            var statusCode = response.ApiCallDetails?.HttpStatusCode;

            if (statusCode is >= 200 and < 300)
            {
                return;
            }

            throw new InvalidOperationException(message);
        }
    }
}
