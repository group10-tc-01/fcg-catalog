using FCG.Catalog.Domain.Catalog.Entity.Games;
using FCG.Catalog.Domain.Enum;
using FCG.Catalog.Domain.Repositories.Game;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FCG.Catalog.Infrastructure.SqlServer.Repositories
{
    [ExcludeFromCodeCoverage]

    public class GameRepository : IWriteOnlyGameRepository, IReadOnlyGameRepository
    {
        private readonly FcgCatalogDbContext _fcgDbContext;

        public GameRepository(FcgCatalogDbContext context)
        {
            _fcgDbContext = context;
        }

        public async Task AddAsync(Game game)
        {
            await _fcgDbContext.Games.AddAsync(game);
        }

        public IQueryable<Game?> GetAllWithFilters(string? name = null, GameCategory? category = null, decimal? minPrice = null, decimal? maxPrice = null)
        {
            var query = _fcgDbContext.Games
                .Include(g => g!.Promotions)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(g => g!.Title.Value.Contains(name));

            if (category.HasValue)
                query = query.Where(g => g!.Category == category.Value);

            if (minPrice.HasValue)
                query = query.Where(g => g!.Price.Value >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(g => g!.Price.Value <= maxPrice.Value);

            return query;
        }

        public async Task<Game?> GetByNameAsync(string name)
        {
            var game = await _fcgDbContext.Games.AsNoTracking().FirstOrDefaultAsync(g => g.Title.Value == name);

            return game;
        }

        public async Task<Game?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _fcgDbContext.Games
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
        }

        public async Task<Game?> GetByIdActiveAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _fcgDbContext.Games
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.Id == id && g.IsActive, cancellationToken)
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _fcgDbContext.Games
                .AsNoTracking()
                .AnyAsync(g => g.Id == id, cancellationToken);
        }
    }
}
