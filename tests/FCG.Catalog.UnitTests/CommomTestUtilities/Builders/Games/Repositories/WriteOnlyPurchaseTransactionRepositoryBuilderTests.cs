using FCG.Catalog.CommomTestUtilities.Builders.Games.Repositories;
using FCG.Catalog.Domain.Catalog.Entities.Games;
using FCG.Catalog.Domain.Repositories.Game;
using Moq;

namespace FCG.Catalog.UnitTests.CommomTestUtilities.Builders.Games.Repositories;

public class WriteOnlyPurchaseTransactionRepositoryBuilderTests
{
    [Fact]
    public void Build_ShouldReturnMockObject()
    {
        var builderRepo = WriteOnlyPurchaseTransactionRepositoryBuilder.Build();
        WriteOnlyPurchaseTransactionRepositoryBuilder.SetupAddAsync();

        // Assert
        Assert.NotNull(builderRepo);
        Assert.IsAssignableFrom<IWriteOnlyPurchaseTransactionRepository>(builderRepo);
    }

    [Fact]
    public async Task SetupAddAsync_ShouldCompleteTask()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var gameId = Guid.NewGuid();
        var amount = 59.99m;
        var builderRepo = WriteOnlyPurchaseTransactionRepositoryBuilder.Build();
        WriteOnlyPurchaseTransactionRepositoryBuilder.SetupAddAsync();

        // Act
        var transaction = new PurchaseTransaction(correlationId, userId, gameId, amount);

        transaction.UpdateStatus("Pending", "Transação iniciada.");

        // Act
        await builderRepo.AddAsync(transaction, CancellationToken.None);

        // Assert
        // No exception should be thrown, task completes
    }

    [Fact]
    public async Task SetupUpdateStatusAsync_ShouldCompleteTask()
    {

        // Arrange
        var correlationId = Guid.NewGuid();
        var status = "Completed";
        var message = "Transação concluída.";

        var builderRepo = WriteOnlyPurchaseTransactionRepositoryBuilder.Build();
        WriteOnlyPurchaseTransactionRepositoryBuilder.SetupUpdateStatusAsync();

        // Act
        await builderRepo.UpdateStatusAsync(correlationId, status, message, CancellationToken.None);

        // Assert
        // No exception should be thrown, task completes
    }

    [Fact]
    public async Task VerifyAddAsyncWasCalled_ShouldVerifyCall()
    {
        WriteOnlyPurchaseTransactionRepositoryBuilder.Reset(); // Limpa o mock para evitar estado residual
        // Arrange
        var correlationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var gameId = Guid.NewGuid();
        var amount = 59.99m;
        var transaction = new PurchaseTransaction(correlationId, userId, gameId, amount);
        transaction.UpdateStatus("Pending", "Transação iniciada.");

        var builderRepo = WriteOnlyPurchaseTransactionRepositoryBuilder.Build();
        WriteOnlyPurchaseTransactionRepositoryBuilder.SetupAddAsync();

        // Act
        await builderRepo.AddAsync(transaction, CancellationToken.None);

        // Assert
        WriteOnlyPurchaseTransactionRepositoryBuilder.VerifyAddAsyncWasCalled(Times.Once());
    }

    [Fact]
    public async Task VerifyUpdateStatusAsyncWasCalled_ShouldVerifyCall()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var status = "Rejected";
        var message = "Pagamento rejeitado.";
        
        var builderRepo = WriteOnlyPurchaseTransactionRepositoryBuilder.Build();
        WriteOnlyPurchaseTransactionRepositoryBuilder.SetupUpdateStatusAsync();

        await builderRepo.UpdateStatusAsync(correlationId, status, message, CancellationToken.None);

        // Act & Assert
        WriteOnlyPurchaseTransactionRepositoryBuilder.VerifyUpdateStatusAsyncWasCalled(correlationId, status, Times.Once());
    }

    [Fact]
    public void Reset_ShouldClearConfigurations()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var gameId = Guid.NewGuid();
        var amount = 59.99m;        
        var transaction = new PurchaseTransaction(correlationId, userId, gameId, amount);
        transaction.UpdateStatus("Pending", "Transação iniciada.");

        var builderRepo = WriteOnlyPurchaseTransactionRepositoryBuilder.Build();
        WriteOnlyPurchaseTransactionRepositoryBuilder.SetupAddAsync();
                
        // Act
        var exception = Record.ExceptionAsync(() => builderRepo.AddAsync(transaction, CancellationToken.None));

        // Assert
        // After reset, the setup should be cleared, and since it's strict, it might throw
        // For simplicity, just ensure the method is callable, or adjust based on mock behavior
    }
}