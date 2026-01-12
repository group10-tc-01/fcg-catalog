using FCG.Catalog.Domain.Catalog.Entities.Libraries;
using FCG.Domain.Repositories.LibraryRepository;
using Moq;

namespace FCG.Catalog.CommomTestUtilities.Builders.Libraries.Repositories
{
    public static class WriteOnlyLibraryRepositoryBuilder
    {
        private static readonly Mock<IWriteOnlyLibraryRepository> _mock = new Mock<IWriteOnlyLibraryRepository>();

        public static IWriteOnlyLibraryRepository Build() => _mock.Object;

        public static void Reset() => _mock.Reset();

        public static void SetupAddAsync()
        {
            _mock.Setup(repo => repo.AddAsync(It.IsAny<Library>()))
                .Returns(Task.CompletedTask);
        }

        public static void VerifyAddAsyncWasCalled(Times times)
        {
            _mock.Verify(repo => repo.AddAsync(It.IsAny<Library>()), times);
        }

        public static void VerifyAddAsyncWasCalledWithLibrary(Guid userId, Times times)
        {
            _mock.Verify(repo => repo.AddAsync(It.Is<Library>(l => l.UserId == userId)), times);
        }
    }
}