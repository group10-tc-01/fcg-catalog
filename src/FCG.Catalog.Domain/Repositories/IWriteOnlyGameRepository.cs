using FCG.Catalog.Domain.Catalog.Entity.Games;

namespace FCG.Catalog.Domain.Repositories
{
    public interface IWriteOnlyGameRepository
    {
        public Task AddAsync(Game game);
    
    }
}
