using Bogus;
using FCG.Catalog.Domain.Catalog.Entities.Libraries;
using FCG.Catalog.Domain.Catalog.ValueObjects;

namespace FCG.Catalog.CommomTestUtilities.Builders.Libraries
{
    public class LibraryBuilder
    {
        public Library Build()
        {
            var faker = new Faker();
            return Library.Create(faker.Random.Guid());
        }

        public Library BuildWithUserId(Guid userId)
        {
            return Library.Create(userId);
        }

        public Library BuildWithGame(Guid userId, Guid gameId, decimal purchasePrice)
        {
            var library = Library.Create(userId);
            library.AddGame(gameId, Price.Create(purchasePrice));
            return library;
        }

        public Library BuildWithGames(Guid userId, List<(Guid gameId, decimal price)> games)
        {
            var library = Library.Create(userId);
            foreach (var (gameId, price) in games)
            {
                library.AddGame(gameId, Price.Create(price));
            }
            return library;
        }
    }
}