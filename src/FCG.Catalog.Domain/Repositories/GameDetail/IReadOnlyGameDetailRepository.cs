namespace FCG.Catalog.Domain.Repositories.GameDetail
{
    public interface IReadOnlyGameDetailRepository
    {
        Task<Catalog.Entities.Games.GameDetail?> GetByIdAsync(Guid gameId, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Guid gameId, CancellationToken cancellationToken = default);
        IQueryable<Catalog.Entities.Games.GameDetail> GetAll();
    }
}