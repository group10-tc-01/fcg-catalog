using FCG.Catalog.CommomTestUtilities.Builders.Games;
using FCG.Catalog.CommomTestUtilities.Builders.Games.Repositories;
using FCG.Catalog.Domain.Catalog.Entities.Games;
using FCG.Catalog.Domain.Repositories.Game;

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
        Assert.Equal(game.Title, result!.Title);
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
    public async Task Reset_ShouldClearConfigurations()
    {
        // Arrange
        var id = Guid.NewGuid();
        var game = new GameBuilder().Build();
        ReadOnlyGameRepositoryBuilder.SetupGetByIdAsync(id, game);
        var repository = ReadOnlyGameRepositoryBuilder.Build();

        // Act
        ReadOnlyGameRepositoryBuilder.Reset();
        var result = await repository.GetByIdAsync(id, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}
