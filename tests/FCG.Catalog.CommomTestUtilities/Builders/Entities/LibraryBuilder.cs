using Bogus;
using FCG.Catalog.Domain.Catalog.Entities.LibraryGames;

namespace FCG.Catalog.CommomTestsUtilities.Builders.Entities
{
    public class LibraryBuilder
    {
        public static LibraryGame Build()
        {
            return new Faker<LibraryGame>().CustomInstantiator(f => LibraryGame.Create(f.Random.Guid(), f.Random.Guid(), f.Random.Decimal(1, 100))).Generate();
        }
    }
}

