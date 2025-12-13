using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FCG.Catalog.Domain.Catalog.Entity.Promotions;

namespace FCG.Domain.Repositories.PromotionRepository
{
    public interface IReadOnlyPromotionRepository
    {
        Task<Promotion?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Promotion>> GetByGameIdAsync(Guid gameId, CancellationToken cancellationToken = default);
        Task<bool> ExistsActivePromotionForGameAsync(Guid gameId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    }
}