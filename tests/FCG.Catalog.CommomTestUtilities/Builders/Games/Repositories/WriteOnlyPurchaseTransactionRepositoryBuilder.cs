using FCG.Catalog.Domain.Catalog.Entities.Games;
using FCG.Catalog.Domain.Repositories.Game;
using Moq;

namespace FCG.Catalog.CommomTestUtilities.Builders.Games.Repositories
{
    public static class WriteOnlyPurchaseTransactionRepositoryBuilder
    {
        private static readonly Mock<IWriteOnlyPurchaseTransactionRepository> _mock =
            new(MockBehavior.Strict);

        public static IWriteOnlyPurchaseTransactionRepository Build()
            => _mock.Object;

        public static void Reset()
            => _mock.Reset();

        public static void SetupAddAsync()
        {
            _mock
                .Setup(repo => repo.AddAsync(
                    It.IsAny<PurchaseTransaction>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
        }

        public static void VerifyAddAsyncWasCalled(Times times)
        {
            _mock.Verify(repo => repo.AddAsync(
                    It.IsAny<PurchaseTransaction>(),
                    It.IsAny<CancellationToken>()),
                times);
        }

        public static void SetupUpdateStatusAsync()
        {
            _mock
                .Setup(repo => repo.UpdateStatusAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    It.IsAny<string?>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
        }

        public static void VerifyUpdateStatusAsyncWasCalled(
            Guid correlationId,
            string status,
            Times times)
        {
            _mock.Verify(repo => repo.UpdateStatusAsync(
                    correlationId,
                    status,
                    It.IsAny<string?>(),
                    It.IsAny<CancellationToken>()),
                times);
        }
    }
}