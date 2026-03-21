using FCG.Catalog.Application.UseCases.Libraries.Register;
using FCG.Catalog.Domain.Abstractions;
using FCG.Catalog.Domain.Catalog.Entities.Libraries;
using FCG.Catalog.Domain.Exception;
using FCG.Catalog.Domain.Repositories.Library;
using Moq;

namespace FCG.Catalog.UnitTests.UseCases.Libraries.Register;

public class CreateEmptyLibraryUseCaseTests
{
    private readonly Mock<IReadOnlyLibraryRepository> _mockReadOnlyLibraryRepository;
    private readonly Mock<IWriteOnlyLibraryRepository> _mockWriteOnlyLibraryRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly CreateEmptyLibraryUseCase _useCase;

    public CreateEmptyLibraryUseCaseTests()
    {
        _mockReadOnlyLibraryRepository = new Mock<IReadOnlyLibraryRepository>();
        _mockWriteOnlyLibraryRepository = new Mock<IWriteOnlyLibraryRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _useCase = new CreateEmptyLibraryUseCase(
            _mockReadOnlyLibraryRepository.Object,
            _mockWriteOnlyLibraryRepository.Object,
            _mockUnitOfWork.Object
        );
    }

    [Fact]
    public async Task Handle_WhenLibraryDoesNotExist_ShouldCreateAndSaveLibrary()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new CreateEmptyLibraryCommand(userId);
        _mockReadOnlyLibraryRepository.Setup(repo => repo.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Library?)null);

        // Act
        await _useCase.Handle(command, CancellationToken.None);

        // Assert
        _mockWriteOnlyLibraryRepository
            .Verify(repo => repo.AddAsync(It.Is<Library>(lib => lib.UserId == userId)));
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenLibraryAlreadyExists_ShouldThrowConflictException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new CreateEmptyLibraryCommand(userId);
        var existingLibrary = Library.Create(userId);
        _mockReadOnlyLibraryRepository.Setup(repo => repo.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingLibrary);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ConflictException>(() => _useCase.Handle(command, CancellationToken.None));
        Assert.Contains(userId.ToString(), exception.Message);
        _mockWriteOnlyLibraryRepository.Verify(repo => repo.AddAsync(It.IsAny<Library>()), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}