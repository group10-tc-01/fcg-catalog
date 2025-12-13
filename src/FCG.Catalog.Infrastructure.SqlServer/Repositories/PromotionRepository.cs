using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FCG.Catalog.Domain.Catalog.Entity.Promotions;
using FCG.Catalog.Domain.Repositories.Promotion;
using FCG.Domain.Repositories.PromotionRepository;
using Microsoft.EntityFrameworkCore;

namespace FCG.Catalog.Infrastructure.SqlServer.Repositories
{
    [ExcludeFromCodeCoverage]

    public class PromotionRepository : IReadOnlyPromotionRepository, IWriteOnlyPromotionRepository
    {
        private readonly FcgCatalogDbContext _fcgDbContext;

        public PromotionRepository(FcgCatalogDbContext context)
        {
            _fcgDbContext = context;
        }

        public async Task<Promotion?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _fcgDbContext.Promotions
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Promotion>> GetByGameIdAsync(Guid gameId, CancellationToken cancellationToken = default)
        {
            return await _fcgDbContext.Promotions
                .AsNoTracking()
                .Where(p => p.GameId == gameId)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsActivePromotionForGameAsync(
            Guid gameId,
            DateTime startDate,
            DateTime endDate,
            CancellationToken cancellationToken = default)
        {
            return await _fcgDbContext.Promotions
                .AsNoTracking()
                .AnyAsync(p =>
                        p.GameId == gameId &&
                        p.StartDate <= endDate && p.EndDate >= startDate,
                    cancellationToken);
        }

        public Task AddAsync(Promotion promotion, CancellationToken cancellationToken = default)
        {
            _fcgDbContext.Promotions.Add(promotion);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Promotion promotion, CancellationToken cancellationToken = default)
        {
            _fcgDbContext.Promotions.Update(promotion);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Promotion promotion, CancellationToken cancellationToken = default)
        {
            _fcgDbContext.Promotions.Remove(promotion);
            return Task.CompletedTask;
        }
    }
}
