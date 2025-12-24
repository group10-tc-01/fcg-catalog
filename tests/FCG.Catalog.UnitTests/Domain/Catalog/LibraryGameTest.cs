using FCG.Catalog.CommomTestsUtilities.Builders.Entities;
using FCG.Catalog.Domain.Catalog.Entities.LibraryGames;
using FCG.Catalog.Domain.Enum;
using FCG.Catalog.Domain.Exception;
using FCG.Catalog.Messages;
using FluentAssertions;

namespace FCG.Catalog.UnitTests.Domain.Catalog
{
    public class LibraryGameTests
    {
        [Fact]
        public void Given_ValidParameters_When_CreateLibraryGame_Then_ShouldCreateSuccessfullyAndSetProperties()
        {
            var libraryId = Guid.NewGuid();
            var gameId = Guid.NewGuid();
            var purchasePrice = 29.99m;
            var beforeCreate = DateTime.UtcNow;

            var libraryGame = LibraryGame.Create(libraryId, gameId, purchasePrice);
            var afterCreate = DateTime.UtcNow;

            libraryGame.Should().NotBeNull();
            libraryGame.Id.Should().NotBe(Guid.Empty);
            libraryGame.LibraryId.Should().Be(libraryId);
            libraryGame.GameId.Should().Be(gameId);
            libraryGame.PurchasePrice.Value.Should().Be(purchasePrice);
            libraryGame.PurchaseDate.Should().BeOnOrAfter(beforeCreate);
            libraryGame.PurchaseDate.Should().BeOnOrBefore(afterCreate);
        }

        [Fact]
        public void Given_EmptyLibraryId_When_CreateLibraryGame_Then_ShouldCreateWithEmptyLibraryId()
        {
            var emptyLibraryId = Guid.Empty;
            var gameId = Guid.NewGuid();
            var purchasePrice = 19.99m;

            var libraryGame = LibraryGame.Create(emptyLibraryId, gameId, purchasePrice);

            libraryGame.LibraryId.Should().Be(Guid.Empty);
            libraryGame.GameId.Should().Be(gameId);
            libraryGame.PurchasePrice.Value.Should().Be(purchasePrice);
        }

        [Fact]
        public void Given_NegativePrice_When_CreateLibraryGame_Then_ShouldThrowDomainException()
        {
            var libraryId = Guid.NewGuid();
            var gameId = Guid.NewGuid();
            var negativePrice = -10.0m;

            Action act = () => LibraryGame.Create(libraryId, gameId, negativePrice);
            act.Should().Throw<DomainException>().WithMessage(ResourceMessages.PriceCannotBeNegative);
        }

        [Fact]
        public void Given_HighPrecisionPrice_When_CreateLibraryGame_Then_ShouldMaintainPrecision()
        {
            var libraryId = Guid.NewGuid();
            var gameId = Guid.NewGuid();
            var precisePrice = 29.999999m;

            var libraryGame = LibraryGame.Create(libraryId, gameId, precisePrice);

            libraryGame.PurchasePrice.Value.Should().Be(29.999999m);
        }
        [Fact]
        public void Given_ValidParameters_When_Update_Then_ShouldUpdateGameProperties()
        {
            // Arrange
            var game = GameBuilder.Build();

            var newTitle = "Resident Evil 4 Remake";
            var newDescription = "Survival horror game developed by Capcom.";
            var newPrice = 250.00m;
            var newCategory = GameCategory.Simulation;

            // Act
            game.Update(newTitle, newDescription, newPrice, newCategory);

            // Assert
            game.Title.Value.Should().Be(newTitle);
            game.Description.Should().Be(newDescription);
            game.Price.Value.Should().Be(newPrice);
            game.Category.Should().Be(newCategory);
        }

        [Fact]
        public void Given_InvalidName_When_Update_Then_ShouldThrowDomainException()
        {
            // Arrange
            var game = GameBuilder.Build();

            // Cenário 1: Nome vazio/nulo
            var actNullName = () => game.Update("", "Description", 100, GameCategory.Action);

            // Act & Assert
            actNullName.Should().Throw<DomainException>()
                .WithMessage(ResourceMessages.GameNameIsRequired);
        }

        [Fact]
        public void Given_InvalidPrice_When_Update_Then_ShouldThrowDomainException()
        {
            // Arrange
            var game = GameBuilder.Build();
            var invalidPrice = -10.00m;

            // Act
            var act = () => game.Update("Valid Title", "Description", invalidPrice, GameCategory.Action);

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage(ResourceMessages.GamePriceMustBeGreaterThanZero);
        }

  
    }
}

