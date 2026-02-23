using FCG.Catalog.Domain.Catalog.Entities.Games;
using FCG.Catalog.Domain.Catalog.Entities.Libraries;
using FCG.Catalog.Domain.Catalog.Entities.LibraryGames;
using FCG.Catalog.Domain.Catalog.ValueObjects;
using FCG.Catalog.Domain.Enum;
using FCG.Catalog.Infrastructure.SqlServer;
using FCG.Catalog.Infrastructure.SqlServer.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FCG.Catalog.UnitTests.Infrastructure.SqlServer.Repositories
{
    public class LibraryGameRepositoryTests
    {
        private FcgCatalogDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<FcgCatalogDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new FcgCatalogDbContext(options);
        }

        [Fact]
        public async Task AddAsync_ShouldAddLibraryGame()
        {
            using var context = CreateDbContext();
            var repo = new LibraryGameRepository(context);
            var libraryGame = LibraryGame.Create(Guid.NewGuid(), Guid.NewGuid(), Price.Create(10m));

            await repo.AddAsync(libraryGame, CancellationToken.None);
            context.SaveChanges();

            context.LibraryGames.Should().Contain(libraryGame);
        }

        [Fact]
        public async Task HasGameAsync_ShouldReturnTrue_WhenGameExists()
        {
            using var context = CreateDbContext();
            var userId = Guid.NewGuid();

            var library = Library.Create(userId);
            context.Libraries.Add(library);
            await context.SaveChangesAsync();

            // Cria e adiciona um Game para garantir o relacionamento
            var game = Game.Create("Test Game", "Description", Price.Create(20m), GameCategory.Puzzle);
            context.Games.Add(game);
            await context.SaveChangesAsync();

            var gameId = game.Id;
            var libraryGame = LibraryGame.Create(library.Id, gameId, Price.Create(10m));
            context.LibraryGames.Add(libraryGame);

            await context.SaveChangesAsync();

            var repo = new LibraryGameRepository(context);

            var result = await repo.HasGameAsync(userId, gameId, CancellationToken.None);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task GetByUserIdAsync_ShouldReturnGamesForUser()
        {
            using var context = CreateDbContext();
            var userId = Guid.NewGuid();

            // Cria e adiciona um Game para garantir o relacionamento
            var game = Game.Create("Test Game", "Descrição", Price.Create(20m), GameCategory.Puzzle);
            context.Games.Add(game);
            await context.SaveChangesAsync();

            var gameId = game.Id;
            var libraryGame = LibraryGame.Create(userId, gameId, Price.Create(10m));
            context.LibraryGames.Add(libraryGame);
            await context.SaveChangesAsync();

            var repo = new LibraryGameRepository(context);

            var result = await repo.GetByUserIdAsync(userId, CancellationToken.None);

            result.Should().ContainSingle();
        }
    }
}