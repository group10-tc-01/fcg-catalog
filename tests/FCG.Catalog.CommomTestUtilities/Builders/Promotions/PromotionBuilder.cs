using Bogus;
using FCG.Catalog.Domain.Catalog.Entities.Promotions;
using FCG.Catalog.Domain.Catalog.ValueObjects;

namespace FCG.Catalog.CommomTestUtilities.Builders.Promotions
{
    public class PromotionBuilder
    {
        public Promotion Build()
        {
            var faker = new Faker();
            return Promotion.Create(
                faker.Random.Guid(),
                Discount.Create(faker.Random.Decimal(5, 50)),
                DateTime.UtcNow.AddDays(-5),
                DateTime.UtcNow.AddDays(5)
            );
        }

        public Promotion BuildActivePromotion(Guid gameId, decimal discountPercentage)
        {
            return Promotion.Create(
                gameId,
                Discount.Create(discountPercentage),
                DateTime.UtcNow.AddDays(-5),
                DateTime.UtcNow.AddDays(5)
            );
        }

        public Promotion BuildExpiredPromotion(Guid gameId, decimal discountPercentage)
        {
            return Promotion.Create(
                gameId,
                Discount.Create(discountPercentage),
                DateTime.UtcNow.AddDays(-30),
                DateTime.UtcNow.AddDays(-1)
            );
        }

        public Promotion BuildFuturePromotion(Guid gameId, decimal discountPercentage)
        {
            return Promotion.Create(
                gameId,
                Discount.Create(discountPercentage),
                DateTime.UtcNow.AddDays(5),
                DateTime.UtcNow.AddDays(15)
            );
        }

        public Promotion BuildWithDates(Guid gameId, decimal discountPercentage, DateTime startDate, DateTime endDate)
        {
            return Promotion.Create(
                gameId,
                Discount.Create(discountPercentage),
                startDate,
                endDate
            );
        }

        public List<Promotion> BuildActivePromotions(Guid gameId, int count)
        {
            var promotions = new List<Promotion>();
            var faker = new Faker();

            for (int i = 0; i < count; i++)
            {
                promotions.Add(Promotion.Create(
                    gameId,
                    Discount.Create(faker.Random.Decimal(10, 50)),
                    DateTime.UtcNow.AddDays(-5),
                    DateTime.UtcNow.AddDays(5)
                ));
            }

            return promotions;
        }
    }
}