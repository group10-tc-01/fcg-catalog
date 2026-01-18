namespace FCG.Catalog.Domain.Repositories.LibraryGame
{
    public interface IReadOnlyLibraryGameRepository
    {
        Task<bool> HasGameAsync(Guid userId, Guid gameId, CancellationToken cancellationToken);
        Task<IEnumerable<Catalog.Entities.LibraryGames.LibraryGame>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    }
}
