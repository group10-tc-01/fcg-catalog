using FCG.Catalog.Domain.Abstractions;
using FCG.Catalog.Domain.Catalog.ValueObjects;
using System.Diagnostics.CodeAnalysis;

namespace FCG.Catalog.Domain.Catalog.Entity.Promotions
{
    [ExcludeFromCodeCoverage]

    public sealed class Promotion : BaseEntity
    {
        public Guid GameId { get; private set; }
        public Discount DiscountPercentage { get; private set; } = null!;
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        private Promotion(Guid gameId, Discount discountPercentage, DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate)
            {
                throw new ArgumentException("Datas inválidas");
            }

            GameId = gameId;
            DiscountPercentage = discountPercentage;
            StartDate = startDate;
            EndDate = endDate;
        }

        private Promotion() { }

        public static Promotion Create(Guid gameId, Discount discount, DateTime startDate, DateTime endDate)
        {
            return new Promotion(gameId, discount, startDate, endDate);
        }
    }
}
