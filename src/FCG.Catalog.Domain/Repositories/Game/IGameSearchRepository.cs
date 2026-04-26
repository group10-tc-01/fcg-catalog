using FCG.Catalog.Domain.Models;

namespace FCG.Catalog.Domain.Repositories.Game
{
    public interface IGameSearchRepository
    {
        Task IndexAsync(GameSearch game, CancellationToken cancellationToken = default);

        Task<PagedListResponse<GameSearch>> SearchAsync(
            string term,
            PaginationParams pagination,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(Guid gameId, CancellationToken cancellationToken = default);
    }
}
