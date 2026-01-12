namespace FCG.Catalog.Domain.Repositories.Promotion
{
    public interface IWriteOnlyPromotionRepository
    {
        Task AddAsync(Catalog.Entities.Promotion.Promotion promotion, CancellationToken cancellationToken = default);
        Task UpdateAsync(Catalog.Entities.Promotion.Promotion promotion, CancellationToken cancellationToken = default);
        Task DeleteAsync(Catalog.Entities.Promotion.Promotion promotion, CancellationToken cancellationToken = default);
    }
}
