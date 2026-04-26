using FCG.Catalog.Application.UseCases.Games.ProcessPayment;
using FCG.Catalog.Domain.Abstractions;
using FCG.Catalog.Domain.Catalog.Entities.Libraries;
using FCG.Catalog.Domain.Catalog.Entities.LibraryGames;
using FCG.Catalog.Domain.Catalog.ValueObjects;
using FCG.Catalog.Domain.Exception;
using FCG.Catalog.Domain.Repositories.Game;
using FCG.Catalog.Domain.Repositories.Library;
using FCG.Catalog.Domain.Repositories.LibraryGame;
using Microsoft.Extensions.Logging;
using Moq;

namespace FCG.Catalog.UnitTests.UseCases.Games.ProcessPayment;

public class ProcessPaymentResultUseCaseTests
{
    private readonly Mock<IReadOnlyLibraryRepository> _mockReadOnlyLibraryRepository;
    private readonly Mock<IReadOnlyLibraryGameRepository> _mockReadOnlyLibraryGameRepository;
    private readonly Mock<IWriteOnlyLibraryGameRepository> _mockWriteOnlyLibraryGameRepository;
    private readonly Mock<IWriteOnlyPurchaseTransactionRepository> _mockWriteOnlyPurchaseTransactionRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ILogger<ProcessPaymentResultUseCase>> _mockLogger;
    private readonly ProcessPaymentResultUseCase _useCase;

    public ProcessPaymentResultUseCaseTests()
    {
        _mockReadOnlyLibraryRepository = new Mock<IReadOnlyLibraryRepository>();
        _mockReadOnlyLibraryGameRepository = new Mock<IReadOnlyLibraryGameRepository>();
        _mockWriteOnlyLibraryGameRepository = new Mock<IWriteOnlyLibraryGameRepository>();
        _mockWriteOnlyPurchaseTransactionRepository = new Mock<IWriteOnlyPurchaseTransactionRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockLogger = new Mock<ILogger<ProcessPaymentResultUseCase>>();
        _useCase = new ProcessPaymentResultUseCase(
            _mockReadOnlyLibraryRepository.Object,
            _mockReadOnlyLibraryGameRepository.Object,
            _mockWriteOnlyLibraryGameRepository.Object,
            _mockWriteOnlyPurchaseTransactionRepository.Object,
            _mockUnitOfWork.Object,
            _mockLogger.Object
        );
    }

    [Fact]
    public async Task Handle_WhenPaymentApprovedAndUserDoesNotOwnGame_ShouldUpdateStatusAndAddGameToLibrary()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var gameId = Guid.NewGuid();
        var amount = 59.99m;
        var request = new ProcessPaymentResultInput{
            CorrelationId = correlationId, 
            UserId = userId, 
            GameId = gameId, 
            Amount =  amount, 
            IsApproved = true 
        };

        var valor = Price.Create(amount);
        var library = Library.Create(userId);
        var libraryId = library.Id;
        library.AddGame(gameId, valor);          

        _mockReadOnlyLibraryGameRepository.Setup(repo => repo.HasGameAsync(userId, gameId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _mockReadOnlyLibraryRepository.Setup(repo => repo.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(library);

        // Act
        await _useCase.Handle(request, CancellationToken.None);

        // Assert
        _mockWriteOnlyPurchaseTransactionRepository.Verify(repo => repo.UpdateStatusAsync(correlationId, "Completed", null, It.IsAny<CancellationToken>()), Times.Once);
        _mockWriteOnlyLibraryGameRepository.Verify(repo => repo.AddAsync(It.Is<LibraryGame>(lg => lg.LibraryId == libraryId && lg.GameId == gameId && lg.PurchasePrice == amount), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenPaymentApprovedAndUserAlreadyOwnsGame_ShouldUpdateStatusButNotAddGame()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var gameId = Guid.NewGuid();
        var amount = 59.99m;

        var request = new ProcessPaymentResultInput
        {
            CorrelationId = correlationId,
            UserId = userId,
            GameId = gameId,
            Amount = amount,
            IsApproved = true
        };

        _mockReadOnlyLibraryGameRepository.Setup(repo => repo.HasGameAsync(userId, gameId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        await _useCase.Handle(request, CancellationToken.None);

        // Assert
        _mockWriteOnlyPurchaseTransactionRepository.Verify(repo => repo.UpdateStatusAsync(correlationId, "Completed", null, It.IsAny<CancellationToken>()), Times.Once);
        _mockWriteOnlyLibraryGameRepository.Verify(repo => repo.AddAsync(It.IsAny<LibraryGame>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenPaymentRejected_ShouldUpdateStatusToRejected()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var gameId = Guid.NewGuid();
        var amount = 59.99m;
        var request = new ProcessPaymentResultInput
        {
            CorrelationId = correlationId,
            UserId = userId,
            GameId = gameId,
            Amount = amount,
            IsApproved = false
        };

        // Act
        await _useCase.Handle(request, CancellationToken.None);

        // Assert
        _mockWriteOnlyPurchaseTransactionRepository.Verify(repo => repo.UpdateStatusAsync(correlationId, "Rejected", "Payment rejected", It.IsAny<CancellationToken>()), Times.Once);
        _mockReadOnlyLibraryGameRepository.Verify(repo => repo.HasGameAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenPaymentApprovedAndLibraryNotFound_ShouldThrowNotFoundException()
    {
        var correlationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var gameId = Guid.NewGuid();
        var amount = 59.99m;
        var request = new ProcessPaymentResultInput
        {
            CorrelationId = correlationId,
            UserId = userId,
            GameId = gameId,
            Amount = amount,
            IsApproved = true
        };

        _mockReadOnlyLibraryGameRepository.Setup(repo => repo.HasGameAsync(userId, gameId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockReadOnlyLibraryRepository.Setup(repo => repo.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Library?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _useCase.Handle(request, CancellationToken.None));
        _mockWriteOnlyPurchaseTransactionRepository.Verify(repo => repo.UpdateStatusAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
