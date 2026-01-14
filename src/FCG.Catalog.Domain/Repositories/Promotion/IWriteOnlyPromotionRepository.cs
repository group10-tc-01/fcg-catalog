namespace FCG.Catalog.Domain.Repositories.Promotion
{
    public interface IWriteOnlyPromotionRepository
    {
        Task AddAsync(Catalog.Entities.Promotions.Promotion promotion, CancellationToken cancellationToken = default);
        Task UpdateAsync(Catalog.Entities.Promotions.Promotion promotion, CancellationToken cancellationToken = default);
        Task DeleteAsync(Catalog.Entities.Promotions.Promotion promotion, CancellationToken cancellationToken = default);
    }
}
