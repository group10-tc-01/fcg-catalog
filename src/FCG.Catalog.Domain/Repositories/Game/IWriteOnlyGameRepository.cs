namespace FCG.Catalog.Domain.Repositories.Game
{
    public interface IWriteOnlyGameRepository
    {
        public Task AddAsync(Catalog.Entity.Games.Game game);
    
    }
}
