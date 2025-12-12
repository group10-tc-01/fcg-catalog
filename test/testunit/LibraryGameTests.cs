using Xunit;
using FCG.Catalog.Domain.Catalog.Entities.LibraryGames;
using FCG.Catalog.Domain.Catalog.ValueObjects;
using System;

namespace FCG.Catalog.Tests.Unit
{
    public class LibraryGameTests
    {
        [Fact]
        public void Create_WithValidParameters_ShouldCreateLibraryGame()
        {
            var libraryId = Guid.NewGuid();
            var gameId = Guid.NewGuid();
            var price = Price.Create(9.99m);

            var libraryGame = LibraryGame.Create(libraryId, gameId, price);

            Assert.Equal(libraryId, libraryGame.LibraryId);
            Assert.Equal(gameId, libraryGame.GameId);
            Assert.Equal(9.99m, libraryGame.PurchasePrice.Value);
        }
    }
}
