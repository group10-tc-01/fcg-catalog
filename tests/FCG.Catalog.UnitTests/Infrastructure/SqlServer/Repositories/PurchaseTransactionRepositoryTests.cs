using FCG.Catalog.Domain.Catalog.Entities.Games;
using FCG.Catalog.Infrastructure.SqlServer;
using FCG.Catalog.Infrastructure.SqlServer.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FCG.Catalog.UnitTests.Infrastructure.SqlServer.Repositories
{
    public class PurchaseTransactionRepositoryTests
    {
        private readonly FcgCatalogDbContext _dbContext;
        private readonly PurchaseTransactionRepository _repository;

        public PurchaseTransactionRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<FcgCatalogDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_PurchaseTransaction")
                .Options;
            _dbContext = new FcgCatalogDbContext(options);
            _repository = new PurchaseTransactionRepository(_dbContext);
        }

        [Fact]
        public async Task GetByCorrelationIdAsync_ShouldReturnTransaction_WhenExists()
        {
            // Arrange
            var correlationId = Guid.NewGuid();
            var transaction = new PurchaseTransaction(correlationId, Guid.NewGuid(), Guid.NewGuid(), 10m);
            var data = new[] { transaction };
            var queryable = data.AsQueryable();

            _dbContext.Set<PurchaseTransaction>().Add(transaction);
            _dbContext.SaveChanges();

            // Act
            var result = await _repository.GetByCorrelationIdAsync(correlationId, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(correlationId, result.Id);
        }

        [Fact]
        public async Task AddAsync_ShouldCallAddAsyncOnDbSet()
        {
            // Arrange
            var transaction = new PurchaseTransaction(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 10m);

            // Act
            _dbContext.Set<PurchaseTransaction>().Add(transaction);
            _dbContext.SaveChanges();
            await _repository.AddAsync(transaction, CancellationToken.None);

            // Assert
            var exists = await _dbContext.PurchaseTransactions.AnyAsync(x => x.Id == transaction.Id);
            Assert.True(exists);
        }

        [Fact]
        public async Task UpdateStatusAsync_ShouldUpdateStatusAndCallUpdate()
        {
            // Arrange
            var correlationId = Guid.NewGuid();
            var transaction = new PurchaseTransaction(correlationId, Guid.NewGuid(), Guid.NewGuid(), 10m);
            var status = "Completed";
            var message = "Success";

            _dbContext.Set<PurchaseTransaction>().Add(transaction);
            _dbContext.SaveChanges();

            // Act
            await _repository.UpdateStatusAsync(correlationId, status, message, CancellationToken.None);

            // Assert
            var updatedTransaction = _dbContext.Set<PurchaseTransaction>().Find(correlationId);
            Assert.Equal(status, updatedTransaction?.Status);
            Assert.Equal(message, updatedTransaction?.Message);
        }

        [Fact]
        public async Task UpdateStatusAsync_ShouldNotCallUpdate_WhenTransactionNotFound()
        {
            // Arrange
            var correlationId = Guid.NewGuid();

            // Act
            await _repository.UpdateStatusAsync(correlationId, "AnyStatus", "AnyMessage", CancellationToken.None);

            // Assert
            var transaction = _dbContext.Set<PurchaseTransaction>().Find(correlationId);
            Assert.Null(transaction);
        }
    }
}
