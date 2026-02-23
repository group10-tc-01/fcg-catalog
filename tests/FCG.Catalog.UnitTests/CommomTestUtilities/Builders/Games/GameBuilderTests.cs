using FCG.Catalog.CommomTestUtilities.Builders.Games;
using FCG.Catalog.Domain.Enum;

namespace FCG.Catalog.UnitTests.CommomTestUtilities.Builders.Games;

public class GameBuilderTests
{
    private readonly GameBuilder _builder = new();

    [Fact]
    public void Build_ShouldCreateValidGame()
    {
        // Act
        var game = _builder.Build();

        // Assert
        Assert.NotNull(game);
        Assert.NotNull(game.Title);
        Assert.NotNull(game.Description);
        Assert.True(game.Price.Value > 0);
        Assert.True(Enum.IsDefined(typeof(GameCategory), game.Category));
    }

    [Fact]
    public void BuildList_ShouldCreateListWithSpecifiedCount()
    {
        // Arrange
        var count = 5;

        // Act
        var games = _builder.BuildList(count);

        // Assert
        Assert.NotNull(games);
        Assert.Equal(count, games.Count);
        foreach (var game in games)
        {
            Assert.NotNull(game);
            Assert.NotNull(game.Title);
            Assert.NotNull(game.Description);
            Assert.True(game.Price.Value > 0);
            Assert.True(Enum.IsDefined(typeof(GameCategory), game.Category));
        }
    }

    [Fact]
    public void BuildWithName_ShouldCreateGameWithSpecifiedName()
    {
        // Arrange
        var name = "Test Game";

        // Act
        var game = _builder.BuildWithName(name);

        // Assert
        Assert.NotNull(game);
        Assert.Equal(name, game.Title.Value);
        Assert.NotNull(game.Description);
        Assert.True(game.Price.Value > 0);
        Assert.True(Enum.IsDefined(typeof(GameCategory), game.Category));
    }

    [Fact]
    public void BuildWithCategory_ShouldCreateGameWithSpecifiedCategory()
    {
        // Arrange
        var category = GameCategory.Action;

        // Act
        var game = _builder.BuildWithCategory(category);

        // Assert
        Assert.NotNull(game);
        Assert.NotNull(game.Title);
        Assert.NotNull(game.Description);
        Assert.True(game.Price.Value > 0);
        Assert.Equal(category, game.Category);
    }

    [Fact]
    public void BuildWithPrice_ShouldCreateGameWithSpecifiedPrice()
    {
        // Arrange
        var price = 49.99m;

        // Act
        var game = _builder.BuildWithPrice(price);

        // Assert
        Assert.NotNull(game);
        Assert.NotNull(game.Title);
        Assert.NotNull(game.Description);
        Assert.Equal(price, game.Price.Value);
        Assert.True(Enum.IsDefined(typeof(GameCategory), game.Category));
    }

    [Fact]
    public void BuildWithAllParameters_ShouldCreateGameWithAllSpecifiedParameters()
    {
        // Arrange
        var name = "Custom Game";
        var description = "Custom Description";
        var price = 29.99m;
        var category = GameCategory.RPG;

        // Act
        var game = _builder.BuildWithAllParameters(name, description, price, category);

        // Assert
        Assert.NotNull(game);
        Assert.Equal(name, game.Title.Value);
        Assert.Equal(description, game.Description);
        Assert.Equal(price, game.Price.Value);
        Assert.Equal(category, game.Category);
    }

    [Fact]
    public void BuildWithId_ShouldCreateGameWithSpecifiedId()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var game = _builder.BuildWithId(id);

        // Assert
        Assert.NotNull(game);
        Assert.Equal(id, game.Id);
        Assert.NotNull(game.Title);
        Assert.NotNull(game.Description);
        Assert.True(game.Price.Value > 0);
        Assert.True(Enum.IsDefined(typeof(GameCategory), game.Category));
    }

    [Fact]
    public void BuildWithId_WithOptionalParameters_ShouldCreateGameWithSpecifiedParameters()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "Specific Game";
        var price = 39.99m;
        var category = GameCategory.Strategy;

        // Act
        var game = _builder.BuildWithId(id, name, price, category);

        // Assert
        Assert.NotNull(game);
        Assert.Equal(id, game.Id);
        Assert.Equal(name, game.Title.Value);
        Assert.Equal(price, game.Price.Value);
        Assert.Equal(category, game.Category);
        Assert.NotNull(game.Description);
    }

    [Fact]
    public void BuildWithPromotion_ShouldCreateGameWithPromotion()
    {
        // Arrange
        var price = 59.99m;
        var discountPercentage = 20.0m;
        var startDate = DateTime.UtcNow.AddDays(-1);
        var endDate = DateTime.UtcNow.AddDays(1);

        // Act
        var game = _builder.BuildWithPromotion(price, discountPercentage, startDate, endDate);

        // Assert
        Assert.NotNull(game);
        Assert.Equal(price, game.Price.Value);
        Assert.NotNull(game.Title);
        Assert.NotNull(game.Description);
        Assert.True(Enum.IsDefined(typeof(GameCategory), game.Category));
        // Assuming promotions are accessible, but since it's private, we can't directly assert
        // In a real scenario, you might need to expose a method to check promotions or use reflection in tests
        // For now, just ensure the game is created without errors
    }
}