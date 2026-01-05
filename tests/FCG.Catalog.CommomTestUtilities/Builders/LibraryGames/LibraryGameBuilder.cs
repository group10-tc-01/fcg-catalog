using Bogus;
using FCG.Catalog.Domain.Catalog.Entities.LibraryGames;

namespace FCG.Catalog.CommomTestUtilities.Builders.LibraryGames
{
    public class LibraryGameBuilder
    {
        public static LibraryGame Build()
        {
            return new Faker<LibraryGame>().CustomInstantiator(f => LibraryGame.Create(f.Random.Guid(), f.Random.Guid(), f.Random.Decimal(1, 100))).Generate();
        }
    }
}
