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
            if (Validate(title, price, category))
            {
                Title = title;
                Description = description;
                Price = price;
                Category = category;
            };
            throw new DomainException(ResourceMessages.GameValidateData);
        }
        public static Game Create(string title, string description, Price price, GameCategory category)
        {
            return new Game(title, description, price, category);
        }

        private bool Validate(string title, Price price, GameCategory category)
        {
            switch (true)
            {
                case true when string.IsNullOrWhiteSpace(title):
                    throw new DomainException(ResourceMessages.GameNameIsRequired);

                case true when title.Length < 3:
                    throw new DomainException(ResourceMessages.GameTitleMinLength);

                case true when price is null:
                    throw new DomainException(ResourceMessages.GamePriceMustBeGreaterThanZero);

                case true when category == GameCategory.None:
                case true when !System.Enum.IsDefined(typeof(GameCategory), category):
                    throw new DomainException(ResourceMessages.GameCategoryIsRequired);
            }
        }
    }
}
