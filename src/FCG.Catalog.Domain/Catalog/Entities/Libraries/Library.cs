using FCG.Catalog.Domain.Abstractions;
using FCG.Catalog.Domain.Catalog.Entities.LibraryGames;
using FCG.Catalog.Domain.Catalog.ValueObjects;
using FCG.Catalog.Domain.Exception;
using FCG.Catalog.Messages;

namespace FCG.Catalog.Domain.Catalog.Entities.Libraries
{
    public sealed class Library : BaseEntity
    {
        public Guid UserId { get; private set; }

        private readonly List<LibraryGame> _libraryGames;
        public IReadOnlyCollection<LibraryGame> LibraryGames => _libraryGames.AsReadOnly();

        public Library(Guid userId)
        {
            UserId = userId;
            _libraryGames = new List<LibraryGame>();
        }

        public static Library Create(Guid userId)
        {
            return new Library(userId);
        }

        public void AddGame(Guid gameId, Price purchasePrice)
        {
            var gameAlreadyExists = _libraryGames.Any(lg => lg.GameId == gameId);

            if (gameAlreadyExists)
            {
                throw new DomainException(ResourceMessages.GameNameAlreadyExists);
            }

            var libraryGame = LibraryGame.Create(Id, gameId, purchasePrice);

            _libraryGames.Add(libraryGame);
        }
    }
}
