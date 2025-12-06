using FCG.Catalog.Domain.Catalog.Entity.Games;
using FCG.Catalog.Domain.Enum;

namespace FCG.Catalog.Domain.Repositories.Game
{
    public interface IReadOnlyGameRepository
    {
        Task<Catalog.Entity.Games.Game?> GetByNameAsync(string name);
        Task<Catalog.Entity.Games.Game?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
        IQueryable<Catalog.Entity.Games.Game?> GetAllWithFilters(string? name = null, GameCategory? category = null, decimal? minPrice = null, decimal? maxPrice = null);
    }
}
