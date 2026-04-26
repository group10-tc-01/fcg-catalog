using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.Search;
using Elastic.Clients.Elasticsearch.QueryDsl;
using FCG.Catalog.Domain.Models;
using FCG.Catalog.Domain.Repositories.Game;
using FCG.Catalog.Infrastructure.Elasticsearch.Documents;
using FCG.Catalog.Infrastructure.Elasticsearch.Settings;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;

namespace FCG.Catalog.Infrastructure.Elasticsearch.Repositories
{
    public sealed class GameSearchRepository : IGameSearchRepository
    {
        private readonly ElasticsearchClient _client;
        private readonly string _indexName;

        public GameSearchRepository(ElasticsearchClient client, IOptions<ElasticsearchSettings> settings)
        {
            _client = client;
            _indexName = settings.Value.IndexName;
        }

        public async Task IndexAsync(GameSearch game, CancellationToken cancellationToken = default)
        {
            var document = MapToDocument(game, DateTime.UtcNow);

            var response = await _client.IndexAsync(document, i => i
                .Index(_indexName)
                .Id(game.Id.ToString()),
                cancellationToken);

            if (!response.IsValidResponse)
            {
                throw new InvalidOperationException($"Failed to index game search document '{game.Id}'.");
            }
        }

        public async Task<PagedListResponse<GameSearch>> SearchAsync(
            string term,
            PaginationParams pagination,
            CancellationToken cancellationToken = default)
        {
            var from = (pagination.PageNumber - 1) * pagination.PageSize;

            var response = await _client.SearchAsync<GameSearchDocument>(s => s
                .Indices(_indexName)
                .From(from)
                .Size(pagination.PageSize)
                .TrackTotalHits(new TrackHits(true))
                .Query(q => q
                    .Bool(b => b
                        .Filter(f => f
                            .Term(t => t
                                .Field(Infer.Field<GameSearchDocument>(f => f.IsActive))
                                .Value(true)))
                        .Must(m => m
                            .MultiMatch(mm => mm
                                .Query(term)
                                .Fields(Fields.FromFields(new Field[]
                                {
                                    new Field(nameof(GameSearchDocument.Title).ToLowerInvariant(), boost: 2.0f),
                                    new Field(nameof(GameSearchDocument.Description).ToLowerInvariant())
                                }))
                                .Fuzziness(new Fuzziness("AUTO"))
                                .Type(TextQueryType.BestFields)))))
                .Sort(so => so.Score(sc => sc.Order(SortOrder.Desc))),
                cancellationToken);

            if (!response.IsValidResponse)
            {
                throw new InvalidOperationException("Failed to search games in Elasticsearch.");
            }

            var totalCount = (int)response.Total;

            var items = response.Hits
                .Select(hit =>
                {
                    var model = MapToModel(hit.Source!);
                    model.Score = hit.Score ?? 0d;
                    return model;
                })
                .ToList();

            return new PagedListResponse<GameSearch>(items, totalCount, pagination.PageNumber, pagination.PageSize);
        }

        public async Task DeleteAsync(Guid gameId, CancellationToken cancellationToken = default)
        {
            var response = await _client.DeleteAsync(_indexName, gameId.ToString(), cancellationToken);

            if (response.Result == Result.NotFound)
            {
                return;
            }

            if (!response.IsValidResponse)
            {
                throw new InvalidOperationException($"Failed to delete game search document '{gameId}'.");
            }
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
    }
}
