using Bogus;
using FCG.Catalog.Domain.Catalog.Entities.LibraryGames;
using FCG.Catalog.Domain.Catalog.Entities.Games;
using FCG.Catalog.Domain.Catalog.ValueObjects;

namespace FCG.Catalog.CommomTestUtilities.Builders.LibraryGames
{
    public class LibraryGameBuilder
    {
        public LibraryGame Build()
        {
            return new Faker<LibraryGame>()
                .CustomInstantiator(f => LibraryGame.Create(
                    f.Random.Guid(),
                    f.Random.Guid(),
                    Price.Create(f.Random.Decimal(1, 100))))
                .Generate();
        }

        public LibraryGame BuildWithGame(Game game, decimal purchasePrice)
        {
            var libraryGame = LibraryGame.Create(
                Guid.NewGuid(),
                game.Id,
                Price.Create(purchasePrice)
            );

            var gameProperty = typeof(LibraryGame).GetProperty("Game");
            gameProperty?.SetValue(libraryGame, game);

            return libraryGame;
        }

        public LibraryGame BuildWithGameAndDate(Game game, DateTimeOffset purchaseDate, decimal purchasePrice)
        {
            var libraryGame = LibraryGame.Create(
                Guid.NewGuid(),
                game.Id,
                Price.Create(purchasePrice)
            );

            var gameProperty = typeof(LibraryGame).GetProperty("Game");
            gameProperty?.SetValue(libraryGame, game);

            var purchaseDateProperty = typeof(LibraryGame).GetProperty("PurchaseDate");
            purchaseDateProperty?.SetValue(libraryGame, purchaseDate);

            return libraryGame;
        }

        public LibraryGame BuildWithId(Guid id, Guid libraryId, Guid gameId, decimal purchasePrice)
        {
            var libraryGame = LibraryGame.Create(libraryId, gameId, Price.Create(purchasePrice));

            var idProperty = typeof(LibraryGame).BaseType?.GetProperty("Id");
            idProperty?.SetValue(libraryGame, id);

            return libraryGame;
        }
    }
}