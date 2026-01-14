namespace FCG.Catalog.Domain.Repositories.Promotion
{
    public interface IReadOnlyPromotionRepository
    {
        Task<Catalog.Entities.Promotions.Promotion?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Catalog.Entities.Promotions.Promotion>> GetByGameIdAsync(Guid gameId, CancellationToken cancellationToken = default);
        Task<bool> ExistsActivePromotionForGameAsync(Guid gameId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    }
}