using FCG.Catalog.Domain.Abstractions;
using FCG.Catalog.Domain.Catalog.Entities.LibraryGames;
using FCG.Catalog.Domain.Catalog.Entity.Promotions;
using FCG.Catalog.Domain.Catalog.ValueObjects;
using FCG.Catalog.Domain.Enum;
using FCG.Catalog.Domain.Exception;
using FCG.Catalog.Messages;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace FCG.Catalog.Domain.Catalog.Entity.Games
{

    public sealed class Game : BaseEntity
    {
        public Title Title { get; private set; }
        public string Description { get; private set; } = string.Empty;
        public Price Price { get; private set; } = null!;
        public GameCategory Category { get; private set; }  
        public ICollection<Promotions.Promotion>? Promotions { get; }
        public ICollection<LibraryGame>? LibraryGames { get; }

        private Game() { }

        private Game(string title, string description, Price price, GameCategory category)
        {
            Validate(description, price, title);

            Title = title;
            Description = description;
            Price = price;
            Category = category;
        }
        public static Game Create(string title, string description, Price price, GameCategory category)
        {
            return new Game(title, description, price, category);
        }

        public void Update(string title, string description, decimal price, GameCategory category, DateTime updatedAt)
        {
            Validate(description, price, title);

            Title = title;
            Description = description;
            Price = Price.Create(price);
            Category = category;
            UpdatedAt = updatedAt;
        }
        public Promotion? GetActivePromotion()
        {
            if (Promotions is null || !Promotions.Any())
                return null;

            var today = DateTime.UtcNow;

            return Promotions
                .Where(p => p.StartDate <= today && p.EndDate >= today)
                .OrderByDescending(p => p.DiscountPercentage)
                .FirstOrDefault();
        }

        public decimal CalculateDiscountedPrice(Promotion? activePromotion)
        {
            if (activePromotion is null)
                return Price.Value;

            var discountAmount = Price.Value * (activePromotion.DiscountPercentage / 100m);
            return Price.Value - discountAmount;
        }
        private void Validate(string description, decimal price, string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new DomainException(ResourceMessages.GameNameIsRequired);
            }

            if (price < 0)
            {
                throw new DomainException(ResourceMessages.GamePriceMustBeGreaterThanZero);
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                throw new DomainException(ResourceMessages.GameNameIsRequired);
            }
        }
    }
}
