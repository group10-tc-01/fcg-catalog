using FCG.Catalog.Domain.Abstractions;
using FCG.Catalog.Domain.Catalog.Entities.Games;
using FCG.Catalog.Domain.Catalog.ValueObjects;
using FCG.Catalog.Domain.Exception;
using FCG.Catalog.Messages;
using System;
using System.ComponentModel.DataAnnotations;

namespace FCG.Catalog.Domain.Catalog.Entities.Promotions
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

        public void Update(Guid gameId, Discount discount, DateTime startDate, DateTime endDate)
        {

            Validate(gameId, startDate, endDate);
            
            GameId = gameId;
            DiscountPercentage = discount;
            StartDate = startDate;
            EndDate = endDate;
        }
        private void Validate(Guid gameId, DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate)
            {
                throw new DomainException(ResourceMessages.PromotionEndDateMustBeAfterStartDate);
            }

            if( gameId == Guid.Empty)
            {
                throw new DomainException(ResourceMessages.GameNotFound);
            }
        }
    }
}
