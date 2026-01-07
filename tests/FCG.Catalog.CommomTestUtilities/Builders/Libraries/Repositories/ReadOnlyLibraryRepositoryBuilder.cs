using FCG.Catalog.Domain.Catalog.Entities.Libraries;
using FCG.Catalog.Domain.Repositories.Library;
using Moq;

namespace FCG.Catalog.CommomTestUtilities.Builders.Libraries.Repositories
{
    public static class ReadOnlyLibraryRepositoryBuilder
    {
        private static readonly Mock<IReadOnlyLibraryRepository> _mock = new Mock<IReadOnlyLibraryRepository>();

        public static IReadOnlyLibraryRepository Build() => _mock.Object;

        public static void Reset() => _mock.Reset();

        public static void SetupGetByUserIdAsync(Guid userId, Library? library)
        {
            _mock.Setup(repo => repo.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(library);
        }

        public static void SetupGetByUserIdWithGamesAsync(Guid userId, Library? library)
        {
            _mock.Setup(repo => repo.GetByUserIdWithGamesAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(library);
        }

        public static void VerifyGetByUserIdAsyncWasCalled(Guid userId, Times times)
        {
            _mock.Verify(repo => repo.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()), times);
        }

        public static void VerifyGetByUserIdWithGamesAsyncWasCalled(Guid userId, Times times)
        {
            _mock.Verify(repo => repo.GetByUserIdWithGamesAsync(userId, It.IsAny<CancellationToken>()), times);
        }
    }
}