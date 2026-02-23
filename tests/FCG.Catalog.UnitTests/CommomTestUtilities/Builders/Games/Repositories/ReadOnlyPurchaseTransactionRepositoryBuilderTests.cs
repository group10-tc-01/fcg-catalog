using FCG.Catalog.CommomTestUtilities.Builders.Games.Repositories;
using FCG.Catalog.Domain.Catalog.Entities.Games;
using FCG.Catalog.Domain.Repositories.Game;
using Moq;

namespace FCG.Catalog.UnitTests.CommomTestUtilities.Builders.Games.Repositories;

public class ReadOnlyPurchaseTransactionRepositoryBuilderTests
{
    [Fact]
    public void Build_ShouldReturnMockObject()
    {
        // Act
        var repository = ReadOnlyPurchaseTransactionRepositoryBuilder.Build();

        // Assert
        Assert.NotNull(repository);
        Assert.IsAssignableFrom<IReadOnlyPurchaseTransactionRepository>(repository);
    }

    [Fact]
    public async Task SetupGetByCorrelationIdAsync_ShouldReturnConfiguredTransaction()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var gameId = Guid.NewGuid();
        var amount = 59.99m;
        var transaction = new PurchaseTransaction(correlationId, userId, gameId, amount);
        transaction.UpdateStatus("Completed", "Transação concluída.");

        var repository = ReadOnlyPurchaseTransactionRepositoryBuilder.Build();
        ReadOnlyPurchaseTransactionRepositoryBuilder.SetupGetByCorrelationIdAsync(correlationId, transaction);

        // Act
        var result = await repository.GetByCorrelationIdAsync(correlationId, CancellationToken.None);

        // Assert
        Assert.Equal(transaction, result);
    }

    [Fact]
    public async Task SetupGetByCorrelationIdAsync_ShouldReturnNull_WhenConfiguredToNull()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        ReadOnlyPurchaseTransactionRepositoryBuilder.SetupGetByCorrelationIdAsync(correlationId, null);
        var repository = ReadOnlyPurchaseTransactionRepositoryBuilder.Build();

        // Act
        var result = await repository.GetByCorrelationIdAsync(correlationId, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task VerifyGetByCorrelationIdAsyncWasCalled_ShouldVerifyCall()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var repository = ReadOnlyPurchaseTransactionRepositoryBuilder.Build();
        ReadOnlyPurchaseTransactionRepositoryBuilder.SetupGetByCorrelationIdAsync(correlationId, null); // Setup para permitir a chamada
        await repository.GetByCorrelationIdAsync(correlationId, CancellationToken.None);

        // Act & Assert
        ReadOnlyPurchaseTransactionRepositoryBuilder.VerifyGetByCorrelationIdAsyncWasCalled(correlationId, Times.Once());
    }

    [Fact]
    public async Task Reset_ShouldClearConfigurations()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var gameId = Guid.NewGuid();
        var amount = 59.99m;
        var transaction = new PurchaseTransaction(correlationId, userId, gameId, amount);
        transaction.UpdateStatus("Completed", "Transação concluída.");

        var repository = ReadOnlyPurchaseTransactionRepositoryBuilder.Build();
        ReadOnlyPurchaseTransactionRepositoryBuilder.SetupGetByCorrelationIdAsync(correlationId, transaction);
        ReadOnlyPurchaseTransactionRepositoryBuilder.Reset();
        ReadOnlyPurchaseTransactionRepositoryBuilder.SetupGetByCorrelationIdAsync(correlationId, null);

        // Act
        var result = await repository.GetByCorrelationIdAsync(correlationId, CancellationToken.None);

        // Assert
        Assert.Null(result); // After reset, should return default (null)
    }
}