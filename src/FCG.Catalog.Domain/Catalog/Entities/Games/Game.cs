using FCG.Catalog.Domain.Abstractions;
using FCG.Catalog.Domain.Catalog.Entities.LibraryGames;
using FCG.Catalog.Domain.Catalog.ValueObjects;
using FCG.Catalog.Domain.Enum;
using System.Diagnostics.CodeAnalysis;
using FCG.Catalog.Domain.Exception;
using FCG.Catalog.Messages;

namespace FCG.Catalog.Domain.Catalog.Entity.Games
{
    [ExcludeFromCodeCoverage]

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
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new DomainException(ResourceMessages.GameNameIsRequired);
            }

            Title = title;
            Description = description;
            Price = price;
            Category = category;
        }
        public static Game Create(string title, string description, Price price, GameCategory category)
        {
            return new Game(title, description, price, category);
        }
    }
}
