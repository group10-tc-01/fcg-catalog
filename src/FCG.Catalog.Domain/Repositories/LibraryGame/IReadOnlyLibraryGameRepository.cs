using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCG.Catalog.Domain.Repositories.LibraryGame
{
    public interface IReadOnlyLibraryGameRepository
    {
        Task<bool> HasGameAsync(Guid userId, Guid gameId, CancellationToken cancellationToken);
        Task<IEnumerable<Catalog.Entities.LibraryGames.LibraryGame>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    }
}
