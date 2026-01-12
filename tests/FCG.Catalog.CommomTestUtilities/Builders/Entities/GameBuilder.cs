using Bogus;
using FCG.Catalog.Domain.Catalog.Entities.Games;
using FCG.Catalog.Domain.Catalog.ValueObjects;
using FCG.Catalog.Domain.Enum;

namespace FCG.Catalog.CommomTestUtilities.Builders.Entities
{
    public static class GameBuilder
    {
        public static Game Build()
        {
            return new Faker<Game>().CustomInstantiator(f =>
                Game.Create(Title.Create(f.Commerce.ProductName()),
                    f.Commerce.ProductDescription(),
                    Price.Create(f.Random.Decimal(1, 100)),
                    f.PickRandom<GameCategory>()
                )
            ).Generate();
        }
        public static List<Game> BuildList(int count)
        {
            return new Faker<Game>()
                .CustomInstantiator(f => Game.Create(
                    Title.Create(f.Commerce.ProductName()),
                    f.Commerce.ProductDescription(),
                    Price.Create(f.Random.Decimal(1, 100)),
                    f.PickRandom<GameCategory>()
                ))
                .Generate(count);
        }

        public static Game BuildWithName(string name)
        {
            var faker = new Faker();
            return Game.Create(
                Title.Create(name),
                faker.Commerce.ProductDescription(),
                Price.Create(faker.Random.Decimal(1, 100)),
                faker.PickRandom<GameCategory>()
            );
        }

        public static Game BuildWithCategory(GameCategory category)
        {
            var faker = new Faker();
            return Game.Create(
                Title.Create(faker.Commerce.ProductName()),
                faker.Commerce.ProductDescription(),
                Price.Create(faker.Random.Decimal(1, 100)),
                category
            );
        }

        public static Game BuildWithPrice(decimal price)
        {
            var faker = new Faker();
            return Game.Create(
                Title.Create(faker.Commerce.ProductName()),
                faker.Commerce.ProductDescription(),
                Price.Create(price),
                faker.PickRandom<GameCategory>()
            );
        }

        public static Game BuildWithAllParameters(string name, string description, decimal price, GameCategory category)
        {
            return Game.Create(
                Title.Create(name),
                description,
                Price.Create(price),
                category
            );
        }
    }
}