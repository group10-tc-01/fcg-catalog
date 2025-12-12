using Xunit;
using FCG.Catalog.Domain.Catalog.Entity.Libraries;
using FCG.Catalog.Domain.Catalog.ValueObjects;
using System;

namespace FCG.Catalog.Tests.Unit
{
    public class LibraryTests
    {
        [Fact]
        public void Create_WithValidUserId_ShouldCreateLibrary()
        {
            var userId = Guid.NewGuid();
            var library = Library.Create(userId);

            Assert.NotNull(library);
        }

        [Fact]
        public void AddGame_WhenGameAlreadyExists_ShouldThrow()
        {
            var userId = Guid.NewGuid();
            var library = Library.Create(userId);
            var gameId = Guid.NewGuid();

            library.AddGame(gameId, Price.Create(5));

            Assert.ThrowsAny<Exception>(() => library.AddGame(gameId, Price.Create(5)));
        }
    }
}
