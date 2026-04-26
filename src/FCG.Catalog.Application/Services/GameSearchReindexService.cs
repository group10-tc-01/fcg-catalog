using FCG.Catalog.Domain.Repositories.Game;

namespace FCG.Catalog.Application.Services
{
    public sealed class GameSearchReindexService : IGameSearchReindexService
    {
        private readonly IReadOnlyGameRepository _readOnlyGameRepository;
        private readonly IGameSearchRepository _gameSearchRepository;

        public GameSearchReindexService(
            IReadOnlyGameRepository readOnlyGameRepository,
            IGameSearchRepository gameSearchRepository)
        {
            _readOnlyGameRepository = readOnlyGameRepository;
            _gameSearchRepository = gameSearchRepository;
        }

        public async Task ReindexAsync(CancellationToken cancellationToken = default)
        {
            var games = _readOnlyGameRepository
                .GetAllWithFilters()
                .Where(game => game != null)
                .ToList();

            foreach (var game in games)
            {
                await _gameSearchRepository.IndexAsync(
                    GameSearchMapper.ToGameSearch(game!),
                    cancellationToken);
            }
        }
    }
}
