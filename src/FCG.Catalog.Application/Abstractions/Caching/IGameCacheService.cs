using FCG.Catalog.Application.UseCases.Games.Get;
using FCG.Catalog.Application.UseCases.Games.GetById;
using FCG.Catalog.Domain.Models;

namespace FCG.Catalog.Application.Abstractions.Caching
{
    public interface IGameCacheService
    {
        Task<PagedListResponse<GetGameOutput>?> GetGameListAsync(
            GetGameInput request,
            PaginationParams pagination,
            CancellationToken cancellationToken);

        Task SetGameListAsync(
            GetGameInput request,
            PaginationParams pagination,
            PagedListResponse<GetGameOutput> response,
            CancellationToken cancellationToken);

        Task<GetGameIdOutput?> GetGameByIdAsync(Guid gameId, CancellationToken cancellationToken);

        Task SetGameByIdAsync(Guid gameId, GetGameIdOutput response, CancellationToken cancellationToken);

        Task InvalidateGameListAsync(CancellationToken cancellationToken);

        Task InvalidateGameByIdAsync(Guid gameId, CancellationToken cancellationToken);
    }
}
