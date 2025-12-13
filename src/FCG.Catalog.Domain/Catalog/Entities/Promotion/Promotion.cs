using System;
using FCG.Catalog.Domain.Abstractions;
using FCG.Catalog.Domain.Catalog.Entity.Games;
using FCG.Catalog.Domain.Catalog.ValueObjects;
using FCG.Catalog.Domain.Exception;
using FCG.Catalog.Messages;

namespace FCG.Catalog.Domain.Catalog.Entity.Promotions
{
    public sealed class Promotion : BaseEntity
    {
        public Guid GameId { get; private set; }
        public Discount DiscountPercentage { get; private set; } = null!;
        public Game? Game { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        private Promotion(Guid gameId, Discount discountPercentage, DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate)
            {
                throw new DomainException(ResourceMessages.PromotionEndDateMustBeAfterStartDate);
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
