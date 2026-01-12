using FCG.Catalog.Domain.Catalog.Entities.Libraries;
using FCG.Catalog.Domain.Repositories.Library;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace FCG.Catalog.Infrastructure.SqlServer.Repositories
{
    [ExcludeFromCodeCoverage]

    public class LibraryRepository : IWriteOnlyLibraryRepository, IReadOnlyLibraryRepository
    {
        private readonly FcgCatalogDbContext _fcgDbContext;

        public LibraryRepository(FcgCatalogDbContext context)
        {
            _fcgDbContext = context;
        }

        public async Task AddAsync(Library library)
        {
            await _fcgDbContext.Libraries.AddAsync(library);
        }

        public async Task<Library?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _fcgDbContext.Libraries.FirstOrDefaultAsync(l => l.UserId == userId, cancellationToken);
        }

        public async Task<Library?> GetByUserIdWithGamesAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _fcgDbContext.Libraries
                .Include(l => l.LibraryGames)
                .ThenInclude(lg => lg.Game)
                .FirstOrDefaultAsync(l => l.UserId == userId, cancellationToken);
        }
    }
}
