using FCG.Catalog.CommomTestUtilities.Builders.Games.Repositories;
using FCG.Catalog.CommomTestUtilities.Builders.Libraries.Repositories;
using FCG.Catalog.Domain.Catalog.Entities.Libraries;
using FCG.Catalog.Domain.Repositories.Library;
using Moq;

namespace FCG.Catalog.UnitTests.CommomTestUtilities.Builders.Libraries.Repositories;

public class ReadOnlyLibraryRepositoryBuilderTests
{
    [Fact]
    public void Build_ShouldReturnMockObject()
    {
        // Act
        var repository = ReadOnlyLibraryRepositoryBuilder.Build();

        // Assert
        Assert.NotNull(repository);
        Assert.IsAssignableFrom<IReadOnlyLibraryRepository>(repository);
    }

    [Fact]
    public async Task SetupGetByUserIdAsync_ShouldReturnConfiguredLibrary()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var library = Library.Create(userId);
        var repository = ReadOnlyLibraryRepositoryBuilder.Build();

        ReadOnlyLibraryRepositoryBuilder.SetupGetByUserIdAsync(userId, library);

        // Act
        var result = await repository.GetByUserIdAsync(userId, CancellationToken.None);

        // Assert
        Assert.Equal(library, result);
    }

    [Fact]
    public async Task SetupGetByUserIdAsync_ShouldReturnNull_WhenConfiguredToNull()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var repository = ReadOnlyLibraryRepositoryBuilder.Build();

        ReadOnlyLibraryRepositoryBuilder.SetupGetByUserIdAsync(userId, null);

        // Act
        var result = await repository.GetByUserIdAsync(userId, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task SetupGetByUserIdWithGamesAsync_ShouldReturnConfiguredLibrary()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var library = Library.Create(userId);
        var repository = ReadOnlyLibraryRepositoryBuilder.Build();

        ReadOnlyLibraryRepositoryBuilder.SetupGetByUserIdWithGamesAsync(userId, library);

        // Act
        var result = await repository.GetByUserIdWithGamesAsync(userId, CancellationToken.None);

        // Assert
        Assert.Equal(library, result);
    }

    [Fact]
    public async Task VerifyGetByUserIdAsyncWasCalled_ShouldVerifyCall()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var repository = ReadOnlyLibraryRepositoryBuilder.Build();
        
        await repository.GetByUserIdAsync(userId, CancellationToken.None);

        // Act & Assert
        ReadOnlyLibraryRepositoryBuilder.VerifyGetByUserIdAsyncWasCalled(userId, Times.Once());
    }

    [Fact]
    public async Task VerifyGetByUserIdWithGamesAsyncWasCalled_ShouldVerifyCall()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var repository = ReadOnlyLibraryRepositoryBuilder.Build();
        ReadOnlyLibraryRepositoryBuilder.SetupGetByUserIdWithGamesAsync(userId, null); // Permite a chamada

        await repository.GetByUserIdWithGamesAsync(userId, CancellationToken.None);

        // Act & Assert
        ReadOnlyLibraryRepositoryBuilder.VerifyGetByUserIdWithGamesAsyncWasCalled(userId, Times.Once());
    }

    [Fact]
    public void VerifyGetByUserIdWithGamesAsyncWasNeverCalled_ShouldVerifyNeverCalled()
    {
        // Arrange
        var repository = ReadOnlyLibraryRepositoryBuilder.Build();

        // Act & Assert
        ReadOnlyLibraryRepositoryBuilder.VerifyGetByUserIdWithGamesAsyncWasNeverCalled();
    }

    [Fact]
    public void Reset_ShouldClearConfigurations()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var library = Library.Create(userId);
        var repository = ReadOnlyLibraryRepositoryBuilder.Build();

        ReadOnlyLibraryRepositoryBuilder.SetupGetByUserIdAsync(userId, library);

        // Act
        ReadOnlyLibraryRepositoryBuilder.Reset();
        var result = repository.GetByUserIdAsync(userId, CancellationToken.None).Result;

        // Assert
        Assert.Null(result); // After reset, should return default (null)
    }
}