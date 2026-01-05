namespace FCG.Catalog.Domain.Repositories.Library
{
    public interface IReadOnlyLibraryRepository
    {
        Task<Catalog.Entities.Libraries.Library?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<Catalog.Entities.Libraries.Library?> GetByUserIdWithGamesAsync(Guid userId, CancellationToken cancellationToken);
    }
}