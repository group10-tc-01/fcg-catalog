using System; 
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FCG.Catalog.Domain.Catalog.Entities.LibraryGames;
using FCG.Catalog.Domain.Repositories.LibraryGame;
using Microsoft.EntityFrameworkCore;

namespace FCG.Catalog.Infrastructure.SqlServer.Repositories
{
    public class LibraryGameRepository : IReadOnlyLibraryGameRepository, IWriteOnlyLibraryGameRepository
    {
        private readonly FcgCatalogDbContext _fcgDbContext;

        public LibraryGameRepository(FcgCatalogDbContext fcgDbContext)
        {
            _fcgDbContext = fcgDbContext;

        }
        public async Task AddAsync(LibraryGame libraryGame, CancellationToken cancellationToken)
        {
            await _fcgDbContext.AddAsync(libraryGame, cancellationToken);
        }

        public async Task<bool> HasGameAsync(Guid userId, Guid gameId, CancellationToken cancellationToken)
        {
            return await _fcgDbContext.LibraryGames
                .AsNoTracking()
                .AnyAsync(x => x.Library.UserId == userId && x.GameId == gameId, cancellationToken);
        }

        public async Task<IEnumerable<LibraryGame>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _fcgDbContext.LibraryGames
                .AsNoTracking() 
                .Include(x => x.Game)
                .Where(x => x.LibraryId == userId)
                .ToListAsync(cancellationToken);
        }
    }
}