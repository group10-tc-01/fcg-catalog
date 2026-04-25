using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.Search;
using FCG.Catalog.Domain.Models;
using FCG.Catalog.Infrastructure.Elasticsearch.Repositories;
using FCG.Catalog.Infrastructure.Elasticsearch.Settings;
using FluentAssertions;
using Microsoft.Extensions.Options;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace FCG.Catalog.UnitTests.Infrastructure.Elasticsearch.Repositories
{
    public class GameSearchRepositoryTests
    {
        [Fact]
        public async Task IndexAsync_ShouldSendDocumentWithIdAndIndexedAt()
        {
            var gameId = Guid.NewGuid();

            await using var server = new FakeElasticsearchServer(async (request, response) =>
            {
                request.HttpMethod.Should().Be("PUT");
                request.RawUrl.Should().StartWith($"/games/_doc/{gameId}");

                var payload = Encoding.UTF8.GetBytes($$"""
                    {
                      "_index": "games",
                      "_id": "{{gameId}}",
                      "_version": 1,
                      "result": "created",
                      "_shards": {
                        "total": 1,
                        "successful": 1,
                        "failed": 0
                      },
                      "_seq_no": 0,
                      "_primary_term": 1
                    }
                    """);

                response.StatusCode = (int)HttpStatusCode.Created;
                response.ContentType = "application/json";
                await response.OutputStream.WriteAsync(payload);
            });

            var repository = new GameSearchRepository(CreateClient(server.BaseAddress), CreateOptions());

            await repository.IndexAsync(new GameSearch
            {
                Id = gameId,
                Title = "Halo",
                Description = "Space shooter",
                Price = 199.90m,
                Category = "Shooter",
                DiscountedPrice = 149.90m,
                IsActive = true
            });

            server.RequestBody.Should().NotBeNullOrWhiteSpace();

            using var json = JsonDocument.Parse(server.RequestBody!);
            var root = json.RootElement;

            root.GetProperty("id").GetString().Should().Be(gameId.ToString());
            root.GetProperty("title").GetString().Should().Be("Halo");
            root.GetProperty("discountedPrice").GetDecimal().Should().Be(149.90m);
            root.GetProperty("indexedAt").GetString().Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task SearchAsync_ShouldBuildFuzzyQueryAndMapPagedResponse()
        {
            var gameId = Guid.NewGuid();

            await using var server = new FakeElasticsearchServer(async (request, response) =>
            {
                request.HttpMethod.Should().Be("POST");
                request.RawUrl.Should().StartWith("/games/_search");

                var payload = Encoding.UTF8.GetBytes($$"""
                    {
                      "took": 1,
                      "timed_out": false,
                      "_shards": {
                        "total": 1,
                        "successful": 1,
                        "skipped": 0,
                        "failed": 0
                      },
                      "hits": {
                        "total": {
                          "value": 7,
                          "relation": "eq"
                        },
                        "max_score": 2.0,
                        "hits": [
                          {
                            "_index": "games",
                            "_id": "{{gameId}}",
                            "_score": 2.0,
                            "_source": {
                              "id": "{{gameId}}",
                              "title": "Halo Infinite",
                              "description": "Sci-fi shooter",
                              "price": 249.90,
                              "category": "Shooter",
                              "discountedPrice": 199.90,
                              "isActive": true,
                              "indexedAt": "2026-04-23T18:00:00Z"
                            }
                          },
                          {
                            "_index": "games",
                            "_id": "{{Guid.NewGuid()}}",
                            "_score": 1.7,
                            "_source": {
                              "id": "{{Guid.NewGuid()}}",
                              "title": "Halo Wars",
                              "description": "Strategy spin-off",
                              "price": 99.90,
                              "category": "Strategy",
                              "discountedPrice": 79.90,
                              "isActive": true,
                              "indexedAt": "2026-04-23T18:05:00Z"
                            }
                          }
                        ]
                      }
                    }
                    """);

                response.StatusCode = (int)HttpStatusCode.OK;
                response.ContentType = "application/json";
                await response.OutputStream.WriteAsync(payload);
            });

            var repository = new GameSearchRepository(CreateClient(server.BaseAddress), CreateOptions());

            var result = await repository.SearchAsync("halo", new PaginationParams
            {
                PageNumber = 2,
                PageSize = 5
            });

            server.RequestBody.Should().NotBeNullOrWhiteSpace();

            using (var json = JsonDocument.Parse(server.RequestBody!))
            {
                var root = json.RootElement;
                root.GetProperty("from").GetInt32().Should().Be(5);
                root.GetProperty("size").GetInt32().Should().Be(5);
                root.GetProperty("track_total_hits").GetBoolean().Should().BeTrue();

                var boolQuery = root.GetProperty("query").GetProperty("bool");
                var filterTerm = boolQuery.GetProperty("filter")[0].GetProperty("term").GetProperty("isActive");
                filterTerm.GetProperty("value").GetBoolean().Should().BeTrue();

                var multiMatch = boolQuery.GetProperty("must")[0].GetProperty("multi_match");
                multiMatch.GetProperty("query").GetString().Should().Be("halo");
                multiMatch.GetProperty("fuzziness").GetString().Should().Be("AUTO");
                multiMatch.GetProperty("type").GetString().Should().Be("best_fields");
                multiMatch.GetProperty("fields")[0].GetString().Should().Be("title^2");
                multiMatch.GetProperty("fields")[1].GetString().Should().Be("description");

                var sort = root.GetProperty("sort")[0].GetProperty("_score");
                sort.GetProperty("order").GetString().Should().Be("desc");
            }

            result.TotalCount.Should().Be(7);
            result.CurrentPage.Should().Be(2);
            result.PageSize.Should().Be(5);
            result.TotalPages.Should().Be(2);
            result.Items.Should().HaveCount(2);
            result.Items[0].Title.Should().Be("Halo Infinite");
            result.Items[0].DiscountedPrice.Should().Be(199.90m);
            result.Items[0].Score.Should().Be(2.0);
            result.Items[1].Score.Should().Be(1.7);
        }

        [Fact]
        public async Task DeleteAsync_ShouldIgnoreNotFoundResponses()
        {
            var gameId = Guid.NewGuid();

            await using var server = new FakeElasticsearchServer(async (request, response) =>
            {
                request.HttpMethod.Should().Be("DELETE");
                request.RawUrl.Should().StartWith($"/games/_doc/{gameId}");

                var payload = Encoding.UTF8.GetBytes($$"""
                    {
                      "_index": "games",
                      "_id": "{{gameId}}",
                      "_version": 1,
                      "result": "not_found",
                      "_shards": {
                        "total": 1,
                        "successful": 1,
                        "failed": 0
                      },
                      "_seq_no": 0,
                      "_primary_term": 1
                    }
                    """);

                response.StatusCode = (int)HttpStatusCode.NotFound;
                response.ContentType = "application/json";
                await response.OutputStream.WriteAsync(payload);
            });

            var repository = new GameSearchRepository(CreateClient(server.BaseAddress), CreateOptions());

            var act = async () => await repository.DeleteAsync(gameId);

            await act.Should().NotThrowAsync();
        }

        private static ElasticsearchClient CreateClient(string baseAddress)
        {
            var settings = new ElasticsearchClientSettings(new Uri(baseAddress))
                .DefaultIndex("games");

            return new ElasticsearchClient(settings);
        }

        private static IOptions<ElasticsearchSettings> CreateOptions()
        {
            return Options.Create(new ElasticsearchSettings
            {
                Uri = "http://localhost:9200",
                IndexName = "games"
            });
        }

        private sealed class FakeElasticsearchServer : IAsyncDisposable
        {
            private readonly HttpListener _listener;
            private readonly CancellationTokenSource _cancellationTokenSource = new();
            private readonly Task _processingTask;
            private readonly Func<HttpListenerRequest, HttpListenerResponse, Task> _handler;

            public FakeElasticsearchServer(Func<HttpListenerRequest, HttpListenerResponse, Task> handler)
            {
                _handler = handler;

                var tcpListener = new TcpListener(IPAddress.Loopback, 0);
                tcpListener.Start();
                var port = ((IPEndPoint)tcpListener.LocalEndpoint).Port;
                tcpListener.Stop();

                BaseAddress = $"http://127.0.0.1:{port}";

                _listener = new HttpListener();
                _listener.Prefixes.Add($"{BaseAddress}/");
                _listener.Start();

                _processingTask = ProcessRequestsAsync();
            }

            public string BaseAddress { get; }

            public string? RequestBody { get; private set; }

            public async ValueTask DisposeAsync()
            {
                _cancellationTokenSource.Cancel();

                if (_listener.IsListening)
                {
                    _listener.Stop();
                }

                _listener.Close();

                try
                {
                    await _processingTask;
                }
                catch (HttpListenerException)
                {
                }
                catch (ObjectDisposedException)
                {
                }
            }

            private async Task ProcessRequestsAsync()
            {
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    HttpListenerContext context;

                    try
                    {
                        context = await _listener.GetContextAsync();
                    }
                    catch (HttpListenerException)
                    {
                        break;
                    }
                    catch (ObjectDisposedException)
                    {
                        break;
                    }

                    context.Response.Headers["X-Elastic-Product"] = "Elasticsearch";
                    RequestBody = await ReadRequestBodyAsync(context.Request);
                    await _handler(context.Request, context.Response);
                    context.Response.Close();
                    break;
                }
            }

            private static async Task<string?> ReadRequestBodyAsync(HttpListenerRequest request)
            {
                if (!request.HasEntityBody)
                {
                    return null;
                }

                Stream requestStream = request.InputStream;
                var contentEncoding = request.Headers["Content-Encoding"];

                if (string.Equals(contentEncoding, "gzip", StringComparison.OrdinalIgnoreCase))
                {
                    requestStream = new GZipStream(request.InputStream, CompressionMode.Decompress);
                }
                else if (string.Equals(contentEncoding, "deflate", StringComparison.OrdinalIgnoreCase))
                {
                    requestStream = new DeflateStream(request.InputStream, CompressionMode.Decompress);
                }

                using var reader = new StreamReader(requestStream, request.ContentEncoding);
                return await reader.ReadToEndAsync();
            }
        }
    }
}
