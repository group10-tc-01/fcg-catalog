using System;
using System.Threading;
using System.Threading.Tasks;
using FCG.Catalog.Domain.Catalog.Entity.Libraries;

namespace FCG.Domain.Repositories.LibraryRepository
{
    public interface IReadOnlyLibraryRepository
    {
        Task<Library?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<Library?> GetByUserIdWithGamesAsync(Guid userId, CancellationToken cancellationToken);
    }
}