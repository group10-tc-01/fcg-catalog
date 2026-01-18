using FCG.Catalog.Domain.Catalog.Entities.Promotions;
using FCG.Catalog.Domain.Repositories.Promotion;
using Moq;

namespace FCG.Catalog.CommomTestUtilities.Builders.Promotions.Repositories
{
    public static class ReadOnlyPromotionRepositoryBuilder
    {
        private static readonly Mock<IReadOnlyPromotionRepository> _mock = new Mock<IReadOnlyPromotionRepository>();

        public static IReadOnlyPromotionRepository Build() => _mock.Object;

        public static void Reset() => _mock.Reset();

        public static void SetupGetByIdAsync(Guid id, Promotion? promotion)
        {
            _mock.Setup(repo => repo.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(promotion);
        }

        public static void SetupGetByGameIdAsync(Guid gameId, IEnumerable<Promotion> promotions)
        {
            _mock.Setup(repo => repo.GetByGameIdAsync(gameId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(promotions);
        }

        public static void SetupExistsActivePromotionForGameAsync(Guid gameId, DateTime startDate, DateTime endDate, bool exists)
        {
            _mock.Setup(repo => repo.ExistsActivePromotionForGameAsync(gameId, startDate, endDate, It.IsAny<CancellationToken>()))
                .ReturnsAsync(exists);
        }

        public static void VerifyGetByIdAsyncWasCalled(Guid id, Times times)
        {
            _mock.Verify(repo => repo.GetByIdAsync(id, It.IsAny<CancellationToken>()), times);
        }

        public static void VerifyGetByGameIdAsyncWasCalled(Guid gameId, Times times)
        {
            _mock.Verify(repo => repo.GetByGameIdAsync(gameId, It.IsAny<CancellationToken>()), times);
        }

        public static void VerifyExistsActivePromotionForGameAsyncWasCalled(Guid gameId, Times times)
        {
            _mock.Verify(repo => repo.ExistsActivePromotionForGameAsync(
                gameId,
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>()), times);
        }

        public static void VerifyExistsActivePromotionForGameAsyncWasCalled(Guid gameId, DateTime startDate, DateTime endDate, Times times)
        {
            _mock.Verify(repo => repo.ExistsActivePromotionForGameAsync(
                gameId,
                startDate,
                endDate,
                It.IsAny<CancellationToken>()), times);
        }
    }
}