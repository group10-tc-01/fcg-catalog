using FCG.Catalog.Application.UseCases.Games.GetPurchaseStatus;
using FCG.Catalog.Domain.Catalog.Entities.Games;
using FCG.Catalog.Domain.Repositories.Game;
using MediatR;
using Moq;
using Xunit;

namespace FCG.Catalog.UnitTests.UseCases.Games.GetPurchaseStatus;

public class GetPurchaseStatusHandlerTests
{
    private readonly Mock<IReadOnlyPurchaseTransactionRepository> _mockRepository;
    private readonly GetPurchaseStatusHandler _handler;

    public GetPurchaseStatusHandlerTests()
    {
        _mockRepository = new Mock<IReadOnlyPurchaseTransactionRepository>();
        _handler = new GetPurchaseStatusHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenTransactionDoesNotExist()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var request = new GetPurchaseStatusInput(correlationId);
        _mockRepository.Setup(repo => repo.GetByCorrelationIdAsync(correlationId, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<PurchaseTransaction?>(null));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(correlationId, result.CorrelationId);
        Assert.Equal("NotFound", result.Status);
        Assert.Equal("Transação não encontrada.", result.Message);
    }

    [Fact]
    public async Task Handle_ShouldReturnTransactionData_WhenTransactionExists()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var request = new GetPurchaseStatusInput(correlationId);
        var transaction = new PurchaseTransaction(
            correlationId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            99.99m);

        transaction.UpdateStatus("Completed", "Compra realizada com sucesso.");

        _mockRepository.Setup(repo => repo.GetByCorrelationIdAsync(correlationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(transaction);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(correlationId, result.CorrelationId);
        Assert.Equal("Completed", result.Status);
        Assert.Equal("Compra realizada com sucesso.", result.Message);
    }
}