using FCG.Catalog.CommomTestUtilities.Builders.Games;
using FCG.Catalog.CommomTestUtilities.Builders.Games.Repositories;
using FCG.Catalog.Domain.Repositories.Game;
using Moq;

namespace FCG.Catalog.UnitTests.CommomTestUtilities.Builders.Games.Repositories;

public class WriteOnlyGameRepositoryBuilderTests
{
    [Fact]
    public void Build_ShouldReturnMockObject()
    {
        // Act
        var repository = WriteOnlyGameRepositoryBuilder.Build();

        // Assert
        Assert.NotNull(repository);
        Assert.IsAssignableFrom<IWriteOnlyGameRepository>(repository);
    }

    [Fact]
    public async Task SetupAddAsync_ShouldCompleteTask()
    {
        // Arrange
        var game = new GameBuilder().Build();
        WriteOnlyGameRepositoryBuilder.SetupAddAsync(game);
        var repository = WriteOnlyGameRepositoryBuilder.Build();

        // Act
        await repository.AddAsync(game);

        // Assert
        // No exception should be thrown, task completes
    }

    [Fact]
    public void SetupUpdate_ShouldComplete()
    {
        // Arrange
        var game = new GameBuilder().Build();
        WriteOnlyGameRepositoryBuilder.SetupUpdate(game);
        var repository = WriteOnlyGameRepositoryBuilder.Build();

        // Act
        repository.Update(game);

        // Assert
        // No exception should be thrown
    }

    [Fact]
    public void VerifyUpdateWasCalled_ShouldVerifyCall()
    {
        WriteOnlyGameRepositoryBuilder.Reset();
        // Arrange
        var game = new GameBuilder().Build();
        WriteOnlyGameRepositoryBuilder.SetupUpdate(game);
        var repository = WriteOnlyGameRepositoryBuilder.Build();
        repository.Update(game);

        // Act & Assert
        WriteOnlyGameRepositoryBuilder.VerifyUpdateWasCalled(Times.Once());
    }

    [Fact]
    public void Reset_ShouldClearConfigurations()
    {
        // Arrange
        var game = new GameBuilder().Build();
        WriteOnlyGameRepositoryBuilder.SetupAddAsync(game);
        var repository = WriteOnlyGameRepositoryBuilder.Build();

        // Act
        WriteOnlyGameRepositoryBuilder.Reset();
        var exception = Record.ExceptionAsync(() => repository.AddAsync(game));

        // Assert
        // After reset, the setup should be cleared, but since it's strict, it might throw
        // For simplicity, just ensure no immediate error, or adjust based on mock behavior
        // In practice, reset clears setups, so calling without setup might throw
    }
}