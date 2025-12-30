namespace FCG.Catalog.Domain.Repositories.LibraryGame
{
    public interface IWriteOnlyLibraryGameRepository
    {
        Task AddAsync(Catalog.Entities.LibraryGames.LibraryGame libraryGame, CancellationToken cancellationToken);
    }
}
