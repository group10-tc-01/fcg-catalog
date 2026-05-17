namespace FCG.Catalog.Domain.Repositories.GameDetail
{
    public interface IWriteOnlyGameDetailRepository
    {
        Task AddAsync(Catalog.Entities.Games.GameDetail gameDetail, CancellationToken cancellationToken = default);
        Task UpdateAsync(Catalog.Entities.Games.GameDetail gameDetail, CancellationToken cancellationToken = default);
        Task AddOrUpdateAsync(Catalog.Entities.Games.GameDetail gameDetail, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid gameId, CancellationToken cancellationToken = default);
    }
}
