using FCG.Catalog.Domain.Catalog.Entities.Games;
using FCG.Catalog.Domain.Enum;
using FCG.Catalog.Domain.Repositories.Game;
using Moq;

namespace FCG.Catalog.CommomTestUtilities.Builders.Games.Repositories
{
    public static class ReadOnlyGameRepositoryBuilder
    {
        private static readonly Mock<IReadOnlyGameRepository> _mock = new Mock<IReadOnlyGameRepository>();

        public static IReadOnlyGameRepository Build() => _mock.Object;

        public static void Reset() => _mock.Reset();

        public static void SetupGetByIdAsync(Guid id, Game? game)
        {
            _mock.Setup(repo => repo.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(game);
        }

        public static void SetupGetByIdActiveAsync(Guid id, Game? game)
        {
            _mock.Setup(repo => repo.GetByIdActiveAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(game);
        }

        public static void SetupGetByNameAsync(string name, Game? game)
        {
            _mock.Setup(repo => repo.GetByNameAsync(name))
                .ReturnsAsync(game);
        }

        public static void SetupExistsAsync(Guid id, bool exists)
        {
            _mock.Setup(repo => repo.ExistsAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(exists);
        }

        public static void SetupDelete(Game game)
        {
            _mock.Setup(repo => repo.Delete(game, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
        }

        public static void SetupGetAllWithFilters(
            IQueryable<Game?> games,
            string? name = null,
            GameCategory? category = null,
            decimal? minPrice = null,
            decimal? maxPrice = null)
        {
            _mock.Setup(repo => repo.GetAllWithFilters(name, category, minPrice, maxPrice))
                .Returns(games);
        }

        public static void VerifyGetAllWithFiltersWasCalled(
            Times times,
            string? name = null,
            GameCategory? category = null,
            decimal? minPrice = null,
            decimal? maxPrice = null)
        {
            _mock.Verify(repo => repo.GetAllWithFilters(name, category, minPrice, maxPrice), times);
        }

        public static void VerifyGetByIdAsyncWasCalled(Guid id, Times times)
        {
            _mock.Verify(repo => repo.GetByIdAsync(id, It.IsAny<CancellationToken>()), times);
        }

        public static void VerifyGetByIdActiveAsyncWasCalled(Guid id, Times times)
        {
            _mock.Verify(repo => repo.GetByIdActiveAsync(id, It.IsAny<CancellationToken>()), times);
        }
    }
}