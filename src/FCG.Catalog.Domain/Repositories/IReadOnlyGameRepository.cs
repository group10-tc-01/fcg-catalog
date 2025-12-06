using FCG.Catalog.Domain.Catalog.Entity.Games;
using FCG.Catalog.Domain.Enum;

namespace FCG.Catalog.Domain.Repositories
{
    public interface IReadOnlyGameRepository
    {
        Task<Game?> GetByNameAsync(string name);
        Task<Game?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
        IQueryable<Game?> GetAllWithFilters(string? name = null, GameCategory? category = null, decimal? minPrice = null, decimal? maxPrice = null);
    }
}
