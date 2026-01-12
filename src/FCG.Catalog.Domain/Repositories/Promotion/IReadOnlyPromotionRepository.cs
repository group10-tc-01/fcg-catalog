namespace FCG.Catalog.Domain.Repositories.Promotion
{
    public interface IReadOnlyPromotionRepository
    {
        Task<FCG.Catalog.Domain.Catalog.Entities.Promotion.Promotion?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<FCG.Catalog.Domain.Catalog.Entities.Promotion.Promotion>> GetByGameIdAsync(Guid gameId, CancellationToken cancellationToken = default);
        Task<bool> ExistsActivePromotionForGameAsync(Guid gameId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    }
}