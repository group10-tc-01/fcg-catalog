using FCG.Catalog.Infrastructure.Elasticsearch.DependencyInjection;
using FCG.Catalog.Infrastructure.Elasticsearch.Extensions;
using FCG.Catalog.Infrastructure.Elasticsearch.Settings;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Sockets;
using System.IO.Compression;
using System.Text;

namespace FCG.Catalog.UnitTests.Infrastructure.Elasticsearch.Extensions
{
    public class ElasticsearchBootstrapExtensionsTests
    {
        [Fact]
        public async Task EnsureGamesIndexCreatedAsync_ShouldCreateIndex_WhenMissing()
        {
            await using var server = new FakeElasticsearchServer(indexExists: false);
            var app = CreateApplicationBuilder(server.BaseAddress);

            await app.EnsureGamesIndexCreatedAsync();

            server.Requests.Should().ContainInOrder("HEAD /games", "PUT /games");
            server.PutRequestBody.Should().NotBeNullOrWhiteSpace();
            server.PutRequestBody.Should().Contain("\"number_of_shards\":\"1\"");
            server.PutRequestBody.Should().Contain("\"number_of_replicas\":\"0\"");
            server.PutRequestBody.Should().Contain("\"id\":{\"type\":\"keyword\"}");
            server.PutRequestBody.Should().Contain("\"title\":");
            server.PutRequestBody.Should().Contain("\"keyword\":{\"type\":\"keyword\"}");
            server.PutRequestBody.Should().Contain("\"description\":{\"type\":\"text\"}");
            server.PutRequestBody.Should().Contain("\"category\":{\"type\":\"keyword\"}");
            server.PutRequestBody.Should().Contain("\"price\":{\"scaling_factor\":100");
            server.PutRequestBody.Should().Contain("\"type\":\"scaled_float\"");
            server.PutRequestBody.Should().Contain("\"discountedPrice\":{\"scaling_factor\":100");
            server.PutRequestBody.Should().Contain("\"isActive\":{\"type\":\"boolean\"}");
            server.PutRequestBody.Should().Contain("\"indexedAt\":{\"type\":\"date\"}");
        }

        [Fact]
        public async Task EnsureGamesIndexCreatedAsync_ShouldSkipCreation_WhenIndexAlreadyExists()
        {
            await using var server = new FakeElasticsearchServer(indexExists: true);
            var app = CreateApplicationBuilder(server.BaseAddress);

            await app.EnsureGamesIndexCreatedAsync();

            server.Requests.Should().ContainSingle().Which.Should().Be("HEAD /games");
            server.PutRequestBody.Should().BeNull();
        }

        private static IApplicationBuilder CreateApplicationBuilder(string baseAddress)
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    [$"{ElasticsearchSettings.SectionName}:Uri"] = baseAddress,
                    [$"{ElasticsearchSettings.SectionName}:IndexName"] = "games",
                    [$"{ElasticsearchSettings.SectionName}:Username"] = string.Empty,
                    [$"{ElasticsearchSettings.SectionName}:Password"] = string.Empty
                })
                .Build();

            var services = new ServiceCollection();
            services.AddLogging();
            services.AddElasticsearchInfrastructure(configuration);

            return new ApplicationBuilder(services.BuildServiceProvider());
        }

        private sealed class FakeElasticsearchServer : IAsyncDisposable
        {
            private readonly HttpListener _listener;
            private readonly CancellationTokenSource _cancellationTokenSource = new();
            private readonly Task _processingTask;
            private readonly bool _indexExists;

            public FakeElasticsearchServer(bool indexExists)
            {
                _indexExists = indexExists;

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

            public List<string> Requests { get; } = [];

            public string? PutRequestBody { get; private set; }

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

                    Requests.Add($"{context.Request.HttpMethod} {context.Request.RawUrl}");
                    context.Response.Headers["X-Elastic-Product"] = "Elasticsearch";

                    if (context.Request.HttpMethod == "HEAD" && context.Request.RawUrl == "/games")
                    {
                        context.Response.StatusCode = _indexExists ? (int)HttpStatusCode.OK : (int)HttpStatusCode.NotFound;
                        context.Response.Close();
                        continue;
                    }

                    if (context.Request.HttpMethod == "PUT" && context.Request.RawUrl == "/games")
                    {
                        Stream requestStream = context.Request.InputStream;
                        var contentEncoding = context.Request.Headers["Content-Encoding"];

                        if (string.Equals(contentEncoding, "gzip", StringComparison.OrdinalIgnoreCase))
                        {
                            requestStream = new GZipStream(context.Request.InputStream, CompressionMode.Decompress);
                        }
                        else if (string.Equals(contentEncoding, "deflate", StringComparison.OrdinalIgnoreCase))
                        {
                            requestStream = new DeflateStream(context.Request.InputStream, CompressionMode.Decompress);
                        }

                        using var reader = new StreamReader(requestStream, context.Request.ContentEncoding);
                        PutRequestBody = await reader.ReadToEndAsync();

                        var payload = Encoding.UTF8.GetBytes("{\"acknowledged\":true,\"shards_acknowledged\":true,\"index\":\"games\"}");
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                        context.Response.ContentType = "application/json";
                        await context.Response.OutputStream.WriteAsync(payload);
                        context.Response.Close();
                        continue;
                    }

                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    context.Response.Close();
                }
            }
        }
    }
}
