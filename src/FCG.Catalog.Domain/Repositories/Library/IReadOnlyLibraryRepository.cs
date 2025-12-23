namespace FCG.Catalog.Domain.Repositories.Library
{
    public interface IReadOnlyLibraryRepository
    {
        Task<Catalog.Entity.Libraries.Library?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<Catalog.Entity.Libraries.Library?> GetByUserIdWithGamesAsync(Guid userId, CancellationToken cancellationToken);
    }
}