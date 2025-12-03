using FCG.Catalog.Domain.Abstractions;
using FCG.Catalog.Domain.Catalog.Entities.LibraryGames;
using FCG.Catalog.Domain.Catalog.Entity.Promotions;
using FCG.Catalog.Domain.Catalog.ValueObjects;
using FCG.Catalog.Domain.Enum;
using System.Diagnostics.CodeAnalysis;

namespace FCG.Catalog.Domain.Catalog.Entity.Games
{
    [ExcludeFromCodeCoverage]

    public sealed class Game : BaseEntity
    {
        public string Title { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public Price Price { get; private set; } = null!;
        public GameCategory Category { get; private set; }  
        public ICollection<Promotion>? Promotions { get; }
        public ICollection<LibraryGame>? LibraryGames { get; }

        private Game() { }

        private Game(string title, string description, Price price, GameCategory category)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ("Vai ser implementado");
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
