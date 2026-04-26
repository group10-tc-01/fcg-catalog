using FCG.Catalog.Application.UseCases.Games.Get;
using FCG.Catalog.Application.UseCases.Games.GetById;
using FCG.Catalog.Domain.Models;

namespace FCG.Catalog.Application.Abstractions.Caching
{
    public sealed class NullGameCacheService : IGameCacheService
    {
        public static NullGameCacheService Instance { get; } = new();

        private NullGameCacheService()
        {
        }

        public Task<PagedListResponse<GetGameOutput>?> GetGameListAsync(
            GetGameInput request,
            PaginationParams pagination,
            CancellationToken cancellationToken)
        {
            return Task.FromResult<PagedListResponse<GetGameOutput>?>(null);
        }

        public Task SetGameListAsync(
            GetGameInput request,
            PaginationParams pagination,
            PagedListResponse<GetGameOutput> response,
            CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task<GetGameIdOutput?> GetGameByIdAsync(Guid gameId, CancellationToken cancellationToken)
        {
            return Task.FromResult<GetGameIdOutput?>(null);
        }

        public Task SetGameByIdAsync(Guid gameId, GetGameIdOutput response, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task InvalidateGameListAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task InvalidateGameByIdAsync(Guid gameId, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
