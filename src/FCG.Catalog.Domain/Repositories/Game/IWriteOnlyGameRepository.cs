using System.Threading.Tasks;

namespace FCG.Catalog.Domain.Repositories.Game
{
    public interface IWriteOnlyGameRepository
    {
        public Task AddAsync(Catalog.Entities.Games.Game game);
        public void Update(Catalog.Entities.Games.Game game);

    }
}
