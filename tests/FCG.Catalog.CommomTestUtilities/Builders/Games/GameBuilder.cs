using Bogus;
using FCG.Catalog.Domain.Catalog.Entities.Games;
using FCG.Catalog.Domain.Catalog.Entities.Promotions;
using FCG.Catalog.Domain.Catalog.ValueObjects;
using FCG.Catalog.Domain.Enum;
using System.Reflection;

namespace FCG.Catalog.CommomTestUtilities.Builders.Games
{
    public class GameBuilder
    {
        public Game Build()
        {
            return new Faker<Game>()
                .CustomInstantiator(f => Game.Create(
                    Title.Create(f.Commerce.ProductName()),
                    f.Commerce.ProductDescription(),
                    Price.Create(f.Random.Decimal(1, 100)),
                    f.PickRandom<GameCategory>()
                ))
                .Generate();
        }

        public List<Game> BuildList(int count)
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

        public Game BuildWithName(string name)
        {
            var faker = new Faker();
            return Game.Create(
                Title.Create(name),
                faker.Commerce.ProductDescription(),
                Price.Create(faker.Random.Decimal(1, 100)),
                faker.PickRandom<GameCategory>()
            );
        }

        public Game BuildWithCategory(GameCategory category)
        {
            var faker = new Faker();
            return Game.Create(
                Title.Create(faker.Commerce.ProductName()),
                faker.Commerce.ProductDescription(),
                Price.Create(faker.Random.Decimal(1, 100)),
                category
            );
        }

        public Game BuildWithPrice(decimal price)
        {
            var faker = new Faker();
            return Game.Create(
                Title.Create(faker.Commerce.ProductName()),
                faker.Commerce.ProductDescription(),
                Price.Create(price),
                faker.PickRandom<GameCategory>()
            );
        }

        public Game BuildWithAllParameters(string name, string description, decimal price, GameCategory category)
        {
            return Game.Create(
                Title.Create(name),
                description,
                Price.Create(price),
                category
            );
        }

        public Game BuildWithId(Guid id, string? name = null, decimal? price = null, GameCategory? category = null)
        {
            var faker = new Faker();
            var game = Game.Create(
                Title.Create(name ?? faker.Commerce.ProductName()),
                faker.Commerce.ProductDescription(),
                Price.Create(price ?? faker.Random.Decimal(1, 100)),
                category ?? faker.PickRandom<GameCategory>()
            );

            var idProperty = typeof(Game).BaseType?.GetProperty("Id");
            idProperty?.SetValue(game, id);

            return game;
        }

        public Game BuildWithPromotion(decimal price, decimal discountPercentage, DateTime? startDate = null, DateTime? endDate = null)
        {
            var faker = new Faker();
            var game = Game.Create(
                Title.Create(faker.Commerce.ProductName()),
                faker.Commerce.ProductDescription(),
                Price.Create(price),
                faker.PickRandom<GameCategory>()
            );

            var idProperty = typeof(Game).BaseType?.GetProperty("Id");
            var gameId = (Guid)(idProperty?.GetValue(game) ?? Guid.NewGuid());

            var promotion = Promotion.Create(
                gameId,
                Discount.Create(discountPercentage),
                startDate ?? DateTime.UtcNow.AddDays(-5),
                endDate ?? DateTime.UtcNow.AddDays(5)
            );

            var field = typeof(Game).GetField("_promotions", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field?.GetValue(game) is List<Promotion> list)
            {
                list.Add(promotion);
            }

            return game;
        }
    }
}