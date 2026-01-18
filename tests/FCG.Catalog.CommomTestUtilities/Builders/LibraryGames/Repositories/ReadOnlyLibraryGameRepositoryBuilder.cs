using FCG.Catalog.Domain.Catalog.Entities.LibraryGames;
using FCG.Catalog.Domain.Repositories.LibraryGame;
using Moq;

namespace FCG.Catalog.CommomTestUtilities.Builders.LibraryGames.Repositories
{
    public static class ReadOnlyLibraryGameRepositoryBuilder
    {
        private static readonly Mock<IReadOnlyLibraryGameRepository> _mock = new Mock<IReadOnlyLibraryGameRepository>();

        public static IReadOnlyLibraryGameRepository Build() => _mock.Object;

        public static void Reset() => _mock.Reset();

        public static void SetupHasGameAsync(Guid userId, Guid gameId, bool hasGame)
        {
            _mock.Setup(repo => repo.HasGameAsync(userId, gameId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(hasGame);
        }

        public static void SetupGetByUserIdAsync(Guid userId, IEnumerable<LibraryGame> libraryGames)
        {
            _mock.Setup(repo => repo.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(libraryGames);
        }

        public static void VerifyHasGameAsyncWasCalled(Guid userId, Guid gameId, Times times)
        {
            _mock.Verify(repo => repo.HasGameAsync(userId, gameId, It.IsAny<CancellationToken>()), times);
        }

        public static void VerifyGetByUserIdAsyncWasCalled(Guid userId, Times times)
        {
            _mock.Verify(repo => repo.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()), times);
        }
    }
}