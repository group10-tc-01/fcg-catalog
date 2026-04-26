using System.Globalization;
using System.Text.Json;
using FCG.Catalog.Application.Abstractions.Caching;
using FCG.Catalog.Application.UseCases.Games.Get;
using FCG.Catalog.Application.UseCases.Games.GetById;
using FCG.Catalog.Domain.Models;
using FCG.Catalog.Infrastructure.Redis.Settings;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FCG.Catalog.Infrastructure.Redis.Services
{
    public sealed class GameCacheService : IGameCacheService
    {
        private const string GameListVersionKey = "games:list:version";
        private const string DefaultListVersion = "default";

        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        private readonly IDistributedCache _distributedCache;
        private readonly RedisSettings _settings;
        private readonly ILogger<GameCacheService> _logger;

        public GameCacheService(
            IDistributedCache distributedCache,
            IOptions<RedisSettings> settings,
            ILogger<GameCacheService> logger)
        {
            _distributedCache = distributedCache;
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task<PagedListResponse<GetGameOutput>?> GetGameListAsync(
            GetGameInput request,
            PaginationParams pagination,
            CancellationToken cancellationToken)
        {
            try
            {
                var key = await BuildGameListKeyAsync(request, pagination, cancellationToken);
                var cachedValue = await _distributedCache.GetStringAsync(key, cancellationToken);

                return string.IsNullOrWhiteSpace(cachedValue)
                    ? null
                    : JsonSerializer.Deserialize<PagedListResponse<GetGameOutput>>(cachedValue, JsonOptions);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                _logger.LogWarning(exception, "Failed to read game list from cache.");
                return null;
            }
        }

        public async Task SetGameListAsync(
            GetGameInput request,
            PaginationParams pagination,
            PagedListResponse<GetGameOutput> response,
            CancellationToken cancellationToken)
        {
            try
            {
                var key = await BuildGameListKeyAsync(request, pagination, cancellationToken);
                var value = JsonSerializer.Serialize(response, JsonOptions);

                await _distributedCache.SetStringAsync(key, value, CreateCacheEntryOptions(), cancellationToken);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                _logger.LogWarning(exception, "Failed to write game list to cache.");
            }
        }

        public async Task<GetGameIdOutput?> GetGameByIdAsync(Guid gameId, CancellationToken cancellationToken)
        {
            try
            {
                var cachedValue = await _distributedCache.GetStringAsync(BuildGameDetailKey(gameId), cancellationToken);

                return string.IsNullOrWhiteSpace(cachedValue)
                    ? null
                    : JsonSerializer.Deserialize<GetGameIdOutput>(cachedValue, JsonOptions);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                _logger.LogWarning(exception, "Failed to read game detail {GameId} from cache.", gameId);
                return null;
            }
        }

        public async Task SetGameByIdAsync(Guid gameId, GetGameIdOutput response, CancellationToken cancellationToken)
        {
            try
            {
                var value = JsonSerializer.Serialize(response, JsonOptions);
                await _distributedCache.SetStringAsync(BuildGameDetailKey(gameId), value, CreateCacheEntryOptions(), cancellationToken);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                _logger.LogWarning(exception, "Failed to write game detail {GameId} to cache.", gameId);
            }
        }

        public async Task InvalidateGameListAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _distributedCache.SetStringAsync(GameListVersionKey, Guid.NewGuid().ToString("N"), cancellationToken);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                _logger.LogWarning(exception, "Failed to invalidate game list cache.");
            }
        }

        public async Task InvalidateGameByIdAsync(Guid gameId, CancellationToken cancellationToken)
        {
            try
            {
                await _distributedCache.RemoveAsync(BuildGameDetailKey(gameId), cancellationToken);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                _logger.LogWarning(exception, "Failed to invalidate game detail {GameId} cache.", gameId);
            }
        }

        private async Task<string> BuildGameListKeyAsync(
            GetGameInput request,
            PaginationParams pagination,
            CancellationToken cancellationToken)
        {
            var version = await GetGameListVersionAsync(cancellationToken);
            var category = request.Category?.ToString();
            var minPrice = request.MinPrice?.ToString(CultureInfo.InvariantCulture);
            var maxPrice = request.MaxPrice?.ToString(CultureInfo.InvariantCulture);

            return "games:list:"
                + $"v={Escape(version)}:"
                + $"name={Escape(request.Name)}:"
                + $"category={Escape(category)}:"
                + $"min={Escape(minPrice)}:"
                + $"max={Escape(maxPrice)}:"
                + $"page={pagination.PageNumber}:"
                + $"size={pagination.PageSize}";
        }

        private async Task<string> GetGameListVersionAsync(CancellationToken cancellationToken)
        {
            var version = await _distributedCache.GetStringAsync(GameListVersionKey, cancellationToken);
            return string.IsNullOrWhiteSpace(version) ? DefaultListVersion : version;
        }

        private DistributedCacheEntryOptions CreateCacheEntryOptions()
        {
            return new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(Math.Max(1, _settings.DefaultTtlSeconds))
            };
        }

        private static string BuildGameDetailKey(Guid gameId)
        {
            return $"games:detail:{gameId}";
        }

        private static string Escape(string? value)
        {
            return Uri.EscapeDataString(value ?? string.Empty);
        }
    }
}
