using FCG.Catalog.CommomTestUtilities.Builders.Games;
using FCG.Catalog.CommomTestUtilities.Builders.Games.Repositories;
using FCG.Catalog.Domain.Catalog.Entities.Games;
using FCG.Catalog.Domain.Enum;
using FCG.Catalog.Domain.Repositories.Game;
using Moq;

namespace FCG.Catalog.UnitTests.CommomTestUtilities.Builders.Games.Repositories;

public class ReadOnlyGameRepositoryBuilderTests
{
    [Fact]
    public void Build_ShouldReturnMockObject()
    {
        // Act
        var repository = ReadOnlyGameRepositoryBuilder.Build();

        // Assert
        Assert.NotNull(repository);
        Assert.IsAssignableFrom<IReadOnlyGameRepository>(repository);
    }

    [Fact]
    public async Task SetupGetByIdAsync_ShouldReturnConfiguredGame()
    {
        // Arrange
        var id = Guid.NewGuid();
        var game = new GameBuilder().Build();
        var repository = ReadOnlyGameRepositoryBuilder.Build();

        ReadOnlyGameRepositoryBuilder.SetupGetByIdAsync(id, game);

        // Act
        var result = await repository.GetByIdAsync(id, CancellationToken.None);

        // Assert
        Assert.Equal(game, result);
    }

    [Fact]
    public async Task SetupGetByIdAsync_ShouldReturnNull_WhenConfiguredToNull()
    {
        // Arrange
        var id = Guid.NewGuid();
        var repository = ReadOnlyGameRepositoryBuilder.Build();

        ReadOnlyGameRepositoryBuilder.SetupGetByIdAsync(id, null);

        // Act
        var result = await repository.GetByIdAsync(id, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task SetupGetByIdActiveAsync_ShouldReturnConfiguredGame()
    {
        // Arrange
        var id = Guid.NewGuid();
        var game = new GameBuilder().Build();
        var repository = ReadOnlyGameRepositoryBuilder.Build();

        ReadOnlyGameRepositoryBuilder.SetupGetByIdActiveAsync(id, game);

        // Act
        var result = await repository.GetByIdActiveAsync(id, CancellationToken.None);

        // Assert
        Assert.Equal(game, result);
    }

    [Fact]
    public async Task SetupGetByNameAsync_ShouldReturnConfiguredGame()
    {
        // Arrange
        var name = "Test Game";
        var game = new GameBuilder().BuildWithName(name);
        var repository = ReadOnlyGameRepositoryBuilder.Build();

        ReadOnlyGameRepositoryBuilder.SetupGetByNameAsync(name, game);

        // Act
        var result = await repository.GetByNameAsync(name);

        // Assert
        Assert.Equal(game, result);
    }

    [Fact]
    public async Task SetupExistsAsync_ShouldReturnConfiguredBoolean()
    {
        // Arrange
        var id = Guid.NewGuid();
        var repository = ReadOnlyGameRepositoryBuilder.Build();
        ReadOnlyGameRepositoryBuilder.SetupExistsAsync(id, true);

        // Act
        var result = await repository.ExistsAsync(id, CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task SetupDelete_ShouldCompleteTask()
    {
        // Arrange
        var game = new GameBuilder().Build();
        var repository = ReadOnlyGameRepositoryBuilder.Build();
        ReadOnlyGameRepositoryBuilder.SetupDelete(game);

        // Act
        await repository.Delete(game, CancellationToken.None);

        // Assert
        // No exception should be thrown, task completes
    }

    [Fact]
    public void SetupGetAllWithFilters_ShouldReturnConfiguredQueryable()
    {
        // Arrange
        var games = new List<Game> { new GameBuilder().Build(), new GameBuilder().Build() }.AsQueryable();
        
        var repository = ReadOnlyGameRepositoryBuilder.Build();
        ReadOnlyGameRepositoryBuilder.SetupGetAllWithFilters(games);

        // Act
        var result = repository.GetAllWithFilters();

        // Assert
        Assert.Equal(games, result);
    }

    [Fact]
    public void SetupGetAllWithFilters_WithFilters_ShouldReturnConfiguredQueryable()
    {
        // Arrange
        var games = new List<Game> { new GameBuilder().BuildWithCategory(GameCategory.Action) }.AsQueryable();
        var name = "Action Game";
        var category = GameCategory.Action;
        var minPrice = 10m;
        var maxPrice = 50m;
        var repository = ReadOnlyGameRepositoryBuilder.Build();

        ReadOnlyGameRepositoryBuilder.SetupGetAllWithFilters(games, name, category, minPrice, maxPrice);

        // Act
        var result = repository.GetAllWithFilters(name, category, minPrice, maxPrice);

        // Assert
        Assert.Equal(games, result);
    }

    [Fact]
    public async Task VerifyGetByIdAsyncWasCalled_ShouldVerifyCall()
    {
        // Arrange
        var id = Guid.NewGuid();
        var repository = ReadOnlyGameRepositoryBuilder.Build();
        await repository.GetByIdAsync(id, CancellationToken.None);

        // Act & Assert
        ReadOnlyGameRepositoryBuilder.VerifyGetByIdAsyncWasCalled(id, Times.Once());
    }

    [Fact]
    public async Task VerifyGetByIdActiveAsyncWasCalled_ShouldVerifyCall()
    {
        // Arrange
        var id = Guid.NewGuid();
        var repository = ReadOnlyGameRepositoryBuilder.Build();
        await repository.GetByIdActiveAsync(id, CancellationToken.None);

        // Act & Assert
        ReadOnlyGameRepositoryBuilder.VerifyGetByIdActiveAsyncWasCalled(id, Times.Once());
    }

    [Fact]
    public async Task VerifyExistsAsyncWasCalled_ShouldVerifyCall()
    {
        // Arrange
        var id = Guid.NewGuid();
        var repository = ReadOnlyGameRepositoryBuilder.Build();
        await repository.ExistsAsync(id, CancellationToken.None);

        // Act & Assert
        ReadOnlyGameRepositoryBuilder.VerifyExistsAsyncWasCalled(id, Times.Once());
    }

    [Fact]
    public void Reset_ShouldClearConfigurations()
    {
        // Arrange
        var id = Guid.NewGuid();
        var game = new GameBuilder().Build();
        ReadOnlyGameRepositoryBuilder.SetupGetByIdAsync(id, game);
        var repository = ReadOnlyGameRepositoryBuilder.Build();

        // Act
        ReadOnlyGameRepositoryBuilder.Reset();
        var result = repository.GetByIdAsync(id, CancellationToken.None).Result;

        // Assert
        Assert.Null(result); // After reset, should return default (null)
    }
}