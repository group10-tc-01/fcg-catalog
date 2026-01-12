using FCG.Catalog.Domain.Catalog.Entities.Games;
using FCG.Catalog.Domain.Repositories.Game;
using Moq;

namespace FCG.Catalog.CommomTestUtilities.Builders.Games.Repositories
{
    public static class ReadOnlyPurchaseTransactionRepositoryBuilder
    {
        private static readonly Mock<IReadOnlyPurchaseTransactionRepository> _mock =new(MockBehavior.Strict);

        public static IReadOnlyPurchaseTransactionRepository Build()
            => _mock.Object;

        public static void Reset()
            => _mock.Reset();

        public static void SetupGetByCorrelationIdAsync(
            Guid correlationId,
            PurchaseTransaction? transaction)
        {
            _mock
                .Setup(repo => repo.GetByCorrelationIdAsync(
                    correlationId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(transaction);
        }

        public static void VerifyGetByCorrelationIdAsyncWasCalled(
            Guid correlationId,
            Times times)
        {
            _mock.Verify(repo => repo.GetByCorrelationIdAsync(
                    correlationId,
                    It.IsAny<CancellationToken>()),
                times);
        }
    }
}