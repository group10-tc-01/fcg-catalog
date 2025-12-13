using System;
using FCG.Catalog.Domain.Abstractions;
using FCG.Catalog.Domain.Catalog.Entity.Games;
using FCG.Catalog.Domain.Catalog.Entity.Libraries;
using FCG.Catalog.Domain.Catalog.ValueObjects;
using System.Diagnostics.CodeAnalysis;

namespace FCG.Catalog.Domain.Catalog.Entities.LibraryGames
{
    [ExcludeFromCodeCoverage]
    public sealed class LibraryGame: BaseEntity
    {
        public Guid LibraryId { get; private set; }
        public Guid GameId { get; private set; }
        public DateTime PurchaseDate { get; private set; }
        public Price PurchasePrice { get; private set; } = null!;

        public Library Library { get; private set; } = null!;
        public Game Game { get; private set; } = null!;
        private LibraryGame(Guid libraryId, Guid gameId, Price purchasePrice)
        {
            LibraryId = libraryId;
            GameId = gameId;
            PurchaseDate = DateTime.UtcNow;
            PurchasePrice = purchasePrice;
        }

        private LibraryGame() { }

        public static LibraryGame Create(Guid libraryId, Guid gameId, Price purchasePrice)
        {
            return new LibraryGame(libraryId, gameId, purchasePrice);
        }
    }
}
