using FCG.Catalog.Domain.Catalog.Entities.Promotions;
using FCG.Catalog.Domain.Repositories.Promotion;
using Moq;

namespace FCG.Catalog.CommomTestUtilities.Builders.Promotions.Repositories
{
    public static class WriteOnlyPromotionRepositoryBuilder
    {
        private static readonly Mock<IWriteOnlyPromotionRepository> _mock = new Mock<IWriteOnlyPromotionRepository>();

        public static IWriteOnlyPromotionRepository Build() => _mock.Object;

        public static void Reset() => _mock.Reset();

        // ============= ADD =============
        public static void SetupAddAsync()
        {
            _mock.Setup(repo => repo.AddAsync(It.IsAny<Promotion>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
        }

        public static void VerifyAddAsyncWasCalled(Times times)
        {
            _mock.Verify(repo => repo.AddAsync(It.IsAny<Promotion>(), It.IsAny<CancellationToken>()), times);
        }

        public static void VerifyAddAsyncWasCalledWithPromotion(Guid gameId, Times times)
        {
            _mock.Verify(repo => repo.AddAsync(
                    It.Is<Promotion>(p => p.GameId == gameId),
                    It.IsAny<CancellationToken>()),
                times);
        }

        public static void SetupUpdateAsync()
        {
            _mock.Setup(repo => repo.UpdateAsync(It.IsAny<Promotion>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
        }

        public static void VerifyUpdateAsyncWasCalled(Times times)
        {
            _mock.Verify(repo => repo.UpdateAsync(It.IsAny<Promotion>(), It.IsAny<CancellationToken>()), times);
        }

        public static void VerifyUpdateAsyncWasCalledWithPromotion(Guid gameId, Times times)
        {
            _mock.Verify(repo => repo.UpdateAsync(
                    It.Is<Promotion>(p => p.GameId == gameId),
                    It.IsAny<CancellationToken>()),
                times);
        }

        public static void SetupDeleteAsync()
        {
            _mock.Setup(repo => repo.DeleteAsync(It.IsAny<Promotion>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
        }

        public static void VerifyDeleteAsyncWasCalled(Times times)
        {
            _mock.Verify(repo => repo.DeleteAsync(It.IsAny<Promotion>(), It.IsAny<CancellationToken>()), times);
        }

        public static void VerifyDeleteAsyncWasCalledWithPromotion(Guid gameId, Times times)
        {
            _mock.Verify(repo => repo.DeleteAsync(
                    It.Is<Promotion>(p => p.GameId == gameId),
                    It.IsAny<CancellationToken>()),
                times);
        }
    }
}