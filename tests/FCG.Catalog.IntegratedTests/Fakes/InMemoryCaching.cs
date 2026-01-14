using FCG.Catalog.Infrastructure.Redis.Interface;
using System.Collections.Concurrent;

namespace FCG.Catalog.IntegratedTests.Fakes
{
    public class InMemoryCaching : ICaching
    {
        private readonly ConcurrentDictionary<string, string> _cache = new();

        public Task<string?> GetAsync(string key)
        {
            _cache.TryGetValue(key, out var value);
            return Task.FromResult(value);
        }

        public Task SetAsync(string key, string value)
        {
            _cache[key] = value;
            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key)
        {
            _cache.TryRemove(key, out _);
            return Task.CompletedTask;
        }
    }
}