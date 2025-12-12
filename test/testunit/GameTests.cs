using Xunit;
using FCG.Catalog.Domain.Catalog.Entity.Games;
using FCG.Catalog.Domain.Catalog.ValueObjects;
using FCG.Catalog.Domain.Enum;
using System;

namespace FCG.Catalog.Tests.Unit
{
    public class GameTests
    {
        [Fact]
        public void Create_WithValidParameters_ShouldCreateGame()
        {
            var price = Price.Create(10.5m);
            var game = Game.Create("Game Title", "Description", price, GameCategory.Action);

            Assert.Equal("Game Title", game.Title.Value);
            Assert.Equal(10.5m, game.Price.Value);
            Assert.Equal(GameCategory.Action, game.Category);
        }

        [Fact]
        public void Create_WithInvalidDescription_ShouldThrow()
        {
            Assert.ThrowsAny<Exception>(() => Game.Create("Game Title", "", Price.Create(10), GameCategory.RPG));
        }
    }
}
