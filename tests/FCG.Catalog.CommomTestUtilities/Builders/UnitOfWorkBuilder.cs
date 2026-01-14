using FCG.Catalog.Domain.Abstractions;
using Moq;

namespace FCG.Catalog.CommomTestUtilities.Builders
{
    public static class UnitOfWorkBuilder
    {
        private static readonly Mock<IUnitOfWork> _mock = new Mock<IUnitOfWork>();

        public static IUnitOfWork Build() => _mock.Object;

        public static void Reset() => _mock.Reset();

        public static void SetupSaveChangesAsync(int affectedRows = 1)
        {
            _mock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(affectedRows);
        }

        public static void SetupBeginTransactionAsync()
        {
            _mock.Setup(uow => uow.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
        }

        public static void SetupCommitTransactionAsync()
        {
            _mock.Setup(uow => uow.CommitTransactionAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
        }

        public static void SetupRollbackTransactionAsync()
        {
            _mock.Setup(uow => uow.RollbackTransactionAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
        }

        public static void SetupSaveChangesAsyncThrows(Exception exception)
        {
            _mock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);
        }

        public static void SetupBeginTransactionAsyncThrows(Exception exception)
        {
            _mock.Setup(uow => uow.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);
        }

        public static void SetupCommitTransactionAsyncThrows(Exception exception)
        {
            _mock.Setup(uow => uow.CommitTransactionAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);
        }

        public static void SetupRollbackTransactionAsyncThrows(Exception exception)
        {
            _mock.Setup(uow => uow.RollbackTransactionAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);
        }

        public static void VerifySaveChangesAsyncWasCalled(Times times)
        {
            _mock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), times);
        }

        public static void VerifyBeginTransactionAsyncWasCalled(Times times)
        {
            _mock.Verify(uow => uow.BeginTransactionAsync(It.IsAny<CancellationToken>()), times);
        }

        public static void VerifyCommitTransactionAsyncWasCalled(Times times)
        {
            _mock.Verify(uow => uow.CommitTransactionAsync(It.IsAny<CancellationToken>()), times);
        }

        public static void VerifyRollbackTransactionAsyncWasCalled(Times times)
        {
            _mock.Verify(uow => uow.RollbackTransactionAsync(It.IsAny<CancellationToken>()), times);
        }

        public static void VerifySaveChangesAsyncWasNotCalled()
        {
            _mock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        public static void VerifyBeginTransactionAsyncWasNotCalled()
        {
            _mock.Verify(uow => uow.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        public static void VerifyCommitTransactionAsyncWasNotCalled()
        {
            _mock.Verify(uow => uow.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        public static void VerifyRollbackTransactionAsyncWasNotCalled()
        {
            _mock.Verify(uow => uow.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}