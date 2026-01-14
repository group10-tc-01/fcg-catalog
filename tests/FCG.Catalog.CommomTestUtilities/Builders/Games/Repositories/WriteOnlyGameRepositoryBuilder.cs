using FCG.Catalog.Domain.Catalog.Entities.Games;
using FCG.Catalog.Domain.Repositories.Game;
using Moq;

namespace FCG.Catalog.CommomTestUtilities.Builders.Games.Repositories
{
    public static class WriteOnlyGameRepositoryBuilder
    {
        private static readonly Mock<IWriteOnlyGameRepository> _mock = new Mock<IWriteOnlyGameRepository>();

        public static IWriteOnlyGameRepository Build() => _mock.Object;

        public static void Reset() => _mock.Reset();

        public static void SetupAddAsync(Game game)
        {
            _mock.Setup(repo => repo.AddAsync(game))
                .Returns(Task.CompletedTask);
        }

        public static void SetupUpdate(Game game)
        {
            _mock.Setup(repo => repo.Update(game));
        }

        public static void VerifyAddAsyncWasCalled(Times times)
        {
            _mock.Verify(repo => repo.AddAsync(It.IsAny<Game>()), times);
        }

        public static void VerifyUpdateWasCalled(Times times)
        {
            _mock.Verify(repo => repo.Update(It.IsAny<Game>()), times);
        }

        public static void VerifyAddAsyncWasCalledWith(Game game, Times times)
        {
            _mock.Verify(repo => repo.AddAsync(game), times);
        }
    }
}