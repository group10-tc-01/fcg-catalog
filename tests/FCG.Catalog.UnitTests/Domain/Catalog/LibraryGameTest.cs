using FCG.Catalog.CommomTestUtilities.Builders.Games;
using FCG.Catalog.Domain.Catalog.Entities.LibraryGames;
using FCG.Catalog.Domain.Catalog.ValueObjects;
using FluentAssertions;

namespace FCG.Catalog.UnitTests.Domain.Catalog
{
    public class LibraryGameTests
    {
        [Fact]
        public void Given_ValidParameters_When_Create_Then_ShouldCreateLibraryGameSuccessfully()
        {
            // Arrange
            var libraryId = Guid.NewGuid();
            var gameId = Guid.NewGuid();
            var purchasePrice = Price.Create(59.99m);

            // Act
            var libraryGame = LibraryGame.Create(libraryId, gameId, purchasePrice);

            // Assert
            libraryGame.Should().NotBeNull();
            libraryGame.Id.Should().NotBe(Guid.Empty);
            libraryGame.LibraryId.Should().Be(libraryId);
            libraryGame.GameId.Should().Be(gameId);
            libraryGame.PurchasePrice.Should().Be(purchasePrice);
            libraryGame.PurchaseDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void Given_EmptyLibraryId_When_Create_Then_ShouldCreateWithEmptyLibraryId()
        {
            // Arrange
            var emptyLibraryId = Guid.Empty;
            var gameId = Guid.NewGuid();
            var purchasePrice = Price.Create(49.99m);

            // Act
            var libraryGame = LibraryGame.Create(emptyLibraryId, gameId, purchasePrice);

            // Assert
            libraryGame.LibraryId.Should().Be(Guid.Empty);
            libraryGame.GameId.Should().Be(gameId);
            libraryGame.PurchasePrice.Should().Be(purchasePrice);
        }

        [Fact]
        public void Given_EmptyGameId_When_Create_Then_ShouldCreateWithEmptyGameId()
        {
            // Arrange
            var libraryId = Guid.NewGuid();
            var emptyGameId = Guid.Empty;
            var purchasePrice = Price.Create(39.99m);

            // Act
            var libraryGame = LibraryGame.Create(libraryId, emptyGameId, purchasePrice);

            // Assert
            libraryGame.LibraryId.Should().Be(libraryId);
            libraryGame.GameId.Should().Be(Guid.Empty);
            libraryGame.PurchasePrice.Should().Be(purchasePrice);
        }
    }
}

