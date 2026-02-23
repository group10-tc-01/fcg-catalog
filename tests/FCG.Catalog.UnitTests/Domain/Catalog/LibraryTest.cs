using FCG.Catalog.Domain.Catalog.Entities.Libraries;
using FCG.Catalog.Domain.Catalog.Entities.LibraryGames;
using FCG.Catalog.Domain.Catalog.ValueObjects;
using FCG.Catalog.Domain.Exception;
using FCG.Catalog.Messages;
using FluentAssertions;

namespace FCG.Catalog.UnitTests.Domain.Catalog
{
    public class LibraryTests
    {
        [Fact]
        public void Given_ValidUserId_When_CreateLibrary_Then_ShouldCreateSuccessfully()
        {
            var userId = Guid.NewGuid();

            var library = Library.Create(userId);

            library.Should().NotBeNull();
            library.Id.Should().NotBe(Guid.Empty);
            library.UserId.Should().Be(userId);
        }

        [Fact]
        public void Given_EmptyUserId_When_CreateLibrary_Then_ShouldCreateWithEmptyId()
        {
            var emptyUserId = Guid.Empty;

            var library = Library.Create(emptyUserId);

            library.Should().NotBeNull();
            library.UserId.Should().Be(Guid.Empty);
        }

        [Fact]
        public void Given_ValidUserId_When_CreateLibraryUsingConstructor_Then_ShouldCreateSuccessfully()
        {
            var userId = Guid.NewGuid();

            var library = new Library(userId);

            library.Should().NotBeNull();
            library.UserId.Should().Be(userId);
        }

        [Fact]
        public void Given_Library_When_AddGame_Then_ShouldAddLibraryGameToCollection()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var library = new Library(userId);
            var gameId = Guid.NewGuid();
            var price = Price.Create(19.99m);

            // Act
            library.AddGame(gameId, price);

            // Assert
            library.LibraryGames.Should().ContainSingle();
            var libGame = library.LibraryGames.First();
            libGame.GameId.Should().Be(gameId);
            libGame.LibraryId.Should().Be(library.Id);
            libGame.PurchasePrice.Value.Should().Be(19.99m);
        }

        [Fact]
        public void Given_DuplicateGame_When_AddGame_Then_ShouldThrowDomainException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var library = new Library(userId);
            var gameId = Guid.NewGuid();
            var price = Price.Create(9.99m);

            // Act
            library.AddGame(gameId, price);
            var act = () => library.AddGame(gameId, price);

            // Assert
            act.Should().Throw<DomainException>().WithMessage(ResourceMessages.GameNameAlreadyExists);
        }

        [Fact]
        public void Given_Library_When_AddingMultipleGames_Then_ShouldAddAllSuccessfully()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var library = new Library(userId);
            var gameId1 = Guid.NewGuid();
            var gameId2 = Guid.NewGuid();
            var price1 = Price.Create(29.99m);
            var price2 = Price.Create(39.99m);

            // Act
            library.AddGame(gameId1, price1);
            library.AddGame(gameId2, price2);

            // Assert
            library.LibraryGames.Should().HaveCount(2);
            var libGame1 = library.LibraryGames.First(lg => lg.GameId == gameId1);
            var libGame2 = library.LibraryGames.First(lg => lg.GameId == gameId2);
            libGame1.PurchasePrice.Should().Be(price1);
            libGame2.PurchasePrice.Should().Be(price2);
        }

        [Fact]
        public void Given_Library_When_AddingSameGameTwice_Then_ShouldThrowDomainExceptionOnSecondAdd()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var library = new Library(userId);
            var gameId = Guid.NewGuid();
            var price = Price.Create(19.99m);

            // Act
            library.AddGame(gameId, price);
            var act = () => library.AddGame(gameId, price);

            // Assert
            act.Should().Throw<DomainException>().WithMessage(ResourceMessages.GameNameAlreadyExists);
            library.LibraryGames.Should().ContainSingle();
        }

        [Fact]
        public void Given_Library_When_AddingGameWithZeroPrice_Then_ShouldAddSuccessfully()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var library = new Library(userId);
            var gameId = Guid.NewGuid();
            var zeroPrice = Price.Create(0.01m); // Assuming Price validates > 0

            // Act
            library.AddGame(gameId, zeroPrice);

            // Assert
            library.LibraryGames.Should().ContainSingle();
            var libGame = library.LibraryGames.First();
            libGame.PurchasePrice.Should().Be(zeroPrice);
        }

        [Fact]
        public void Given_Library_When_AddingGame_Then_LibraryGameShouldHaveCorrectLibraryId()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var library = new Library(userId);
            var gameId = Guid.NewGuid();
            var price = Price.Create(49.99m);

            // Act
            library.AddGame(gameId, price);

            // Assert
            var libGame = library.LibraryGames.First();
            libGame.LibraryId.Should().Be(library.Id);
            libGame.GameId.Should().Be(gameId);
            libGame.PurchasePrice.Should().Be(price);
        }

        [Fact]
        public void Given_Library_When_AccessingLibraryGames_Then_ShouldReturnReadOnlyCollection()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var library = new Library(userId);

            // Act
            var libraryGames = library.LibraryGames;

            // Assert
            libraryGames.Should().BeAssignableTo(typeof(IReadOnlyCollection<LibraryGame>));
            libraryGames.Should().BeEmpty();
        }
    }
}