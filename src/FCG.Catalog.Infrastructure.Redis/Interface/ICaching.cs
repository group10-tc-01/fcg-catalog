namespace FCG.Catalog.Infrastructure.Redis.Interface
{
    public interface ICaching
    {
        Task SetAsync(string key, string value);
        Task<string> GetAsync(string key);
        Task RemoveAsync(string key);
    }
}
