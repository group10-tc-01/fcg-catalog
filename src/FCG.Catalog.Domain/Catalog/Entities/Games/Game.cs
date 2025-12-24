using System.Collections.Generic;
using FCG.Catalog.Domain.Abstractions;
using FCG.Catalog.Domain.Catalog.Entities.LibraryGames;
using FCG.Catalog.Domain.Catalog.ValueObjects;
using FCG.Catalog.Domain.Enum;
using FCG.Catalog.Domain.Exception;
using FCG.Catalog.Messages;

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
        public void Update(string title, string description, decimal price, GameCategory category)
        {
            Validate(description, price, title);

            Title = title;
            Description = description;
            Price = Price.Create(price);
            Category = category;
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
