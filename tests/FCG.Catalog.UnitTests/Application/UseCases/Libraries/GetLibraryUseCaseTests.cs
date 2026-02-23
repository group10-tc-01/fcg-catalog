using System.Text.Json;
using FCG.Catalog.Application.UseCases.Libraries.Get;
using FCG.Catalog.CommomTestUtilities.Builders;
using FCG.Catalog.CommomTestUtilities.Builders.Games;
using FCG.Catalog.CommomTestUtilities.Builders.Libraries;
using FCG.Catalog.CommomTestUtilities.Builders.Libraries.Repositories;
using FCG.Catalog.CommomTestUtilities.Builders.LibraryGames;
using FCG.Catalog.Domain.Catalog.Entities.LibraryGames;
using FluentAssertions;
using Moq;

namespace FCG.Catalog.UnitTests.Application.UseCases.Libraries
{
    public class GetLibraryUseCaseTests
    {
        private readonly LibraryBuilder _libraryBuilder;
        private readonly GameBuilder _gameBuilder;

        public GetLibraryUseCaseTests()
        {
            _libraryBuilder = new LibraryBuilder();
            _gameBuilder = new GameBuilder();
            ReadOnlyLibraryRepositoryBuilder.Reset();
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyResponse_WhenLibraryDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var query = new GetLibraryByUserIdQuery(userId);
            var useCase = new GetLibraryUseCase(
                ReadOnlyLibraryRepositoryBuilder.Build()
            );

            ReadOnlyLibraryRepositoryBuilder.SetupGetByUserIdWithGamesAsync(userId, null);

            // Act
            var result = await useCase.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.LibraryId.Should().Be(Guid.Empty);
            result.LibraryGames.Should().NotBeNull();
            result.LibraryGames.Should().BeEmpty();

            ReadOnlyLibraryRepositoryBuilder.VerifyGetByUserIdWithGamesAsyncWasCalled(userId, Times.Once());
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyLibraryGames_WhenLibraryHasNoGames()
        {
            var useCase = new GetLibraryUseCase(
                ReadOnlyLibraryRepositoryBuilder.Build()
            );

            // Arrange
            var userId = Guid.NewGuid();
            var libraryId = Guid.NewGuid();
            var query = new GetLibraryByUserIdQuery(userId);
            var library = _libraryBuilder.BuildWithId(libraryId, userId);

            ReadOnlyLibraryRepositoryBuilder.SetupGetByUserIdWithGamesAsync(userId, library);

            // Act
            var result = await useCase.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.LibraryId.Should().Be(libraryId);
            result.LibraryGames.Should().NotBeNull();
            result.LibraryGames.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_ShouldOrderGamesByPurchaseDateDescending_WhenLibraryHasMultipleGames()
        {
            var useCase = new GetLibraryUseCase(
                ReadOnlyLibraryRepositoryBuilder.Build()
            );

            // Arrange
            var userId = Guid.NewGuid();
            var game1Id = Guid.NewGuid();
            var game2Id = Guid.NewGuid();
            var game3Id = Guid.NewGuid();
            var query = new GetLibraryByUserIdQuery(userId);

            var game1 = _gameBuilder.BuildWithId(game1Id, price: 30.00m);
            var game2 = _gameBuilder.BuildWithId(game2Id, price: 40.00m);
            var game3 = _gameBuilder.BuildWithId(game3Id, price: 50.00m);

            var oldestDate = DateTime.UtcNow.AddDays(-10);
            var middleDate = DateTime.UtcNow.AddDays(-5);
            var newestDate = DateTime.UtcNow;

            var library = _libraryBuilder.BuildWithGames(userId, new List<(Guid, decimal)>
            {
                (game1Id, 30.00m),
                (game2Id, 40.00m),
                (game3Id, 50.00m)
            });

            var libraryGames = library.LibraryGames.ToList();
            var gameProperty = typeof(LibraryGame).GetProperty("Game");
            var dateProperty = typeof(LibraryGame).GetProperty("PurchaseDate");

            gameProperty?.SetValue(libraryGames[0], game1);
            dateProperty?.SetValue(libraryGames[0], oldestDate);

            gameProperty?.SetValue(libraryGames[1], game2);
            dateProperty?.SetValue(libraryGames[1], newestDate);

            gameProperty?.SetValue(libraryGames[2], game3);
            dateProperty?.SetValue(libraryGames[2], middleDate);

            ReadOnlyLibraryRepositoryBuilder.SetupGetByUserIdWithGamesAsync(userId, library);

            // Act
            var result = await useCase.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.LibraryGames.Should().HaveCount(3);
            result.LibraryGames![0].GameId.Should().Be(game2Id); // Mais recente
            result.LibraryGames[1].GameId.Should().Be(game3Id); // Meio
            result.LibraryGames[2].GameId.Should().Be(game1Id); // Mais antigo
        }

        [Fact]
        public async Task Handle_ShouldCacheLibraryData_WhenRetrievedFromDatabase()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var libraryId = Guid.NewGuid();
            var query = new GetLibraryByUserIdQuery(userId);
            var library = _libraryBuilder.BuildWithId(libraryId, userId);

            ReadOnlyLibraryRepositoryBuilder.SetupGetByUserIdWithGamesAsync(userId, library);

            var useCase = new GetLibraryUseCase(
                ReadOnlyLibraryRepositoryBuilder.Build()
            );

            // Act
            await useCase.Handle(query, CancellationToken.None);
        }

        [Fact]
        public async Task Handle_ShouldMapLibraryGameDtoCorrectly_WhenRetrievingFromDatabase()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var gameId = Guid.NewGuid();
            var query = new GetLibraryByUserIdQuery(userId);

            var game = _gameBuilder.BuildWithId(gameId, price: 75.50m);
            var library = _libraryBuilder.BuildWithGame(userId, gameId, 75.50m);

            var purchaseDate = DateTime.UtcNow.AddDays(-3);
            var libraryGames = library.LibraryGames.ToList();
            var gameProperty = typeof(LibraryGame).GetProperty("Game");
            var dateProperty = typeof(LibraryGame).GetProperty("PurchaseDate");

            gameProperty?.SetValue(libraryGames[0], game);
            dateProperty?.SetValue(libraryGames[0], purchaseDate);

            var useCase = new GetLibraryUseCase(
                ReadOnlyLibraryRepositoryBuilder.Build()
            );

            ReadOnlyLibraryRepositoryBuilder.SetupGetByUserIdWithGamesAsync(userId, library);

            // Act
            var result = await useCase.Handle(query, CancellationToken.None);

            // Assert
            var dto = result.LibraryGames![0];
            dto.GameId.Should().Be(gameId);
            dto.Title.Should().Be(game.Title.Value);
            dto.Description.Should().Be(game.Description);
            dto.PurchasePrice.Should().Be(75.50m);
            dto.PurchaseDate.Should().BeCloseTo(purchaseDate, TimeSpan.FromSeconds(1));
        }


        [Fact]
        public async Task Handle_ShouldNotCacheData_WhenLibraryNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var query = new GetLibraryByUserIdQuery(userId);

            ReadOnlyLibraryRepositoryBuilder.SetupGetByUserIdWithGamesAsync(userId, null);

            var useCase = new GetLibraryUseCase(
                ReadOnlyLibraryRepositoryBuilder.Build()
            );

            // Act
            var result = await useCase.Handle(query, CancellationToken.None);

            // Assert
            result.LibraryId.Should().Be(Guid.Empty);
            result.LibraryGames.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_ShouldUseCorrectCacheKey_ForSpecificUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var query = new GetLibraryByUserIdQuery(userId);
            var library = _libraryBuilder.BuildWithUserId(userId);

            ReadOnlyLibraryRepositoryBuilder.SetupGetByUserIdWithGamesAsync(userId, library);

            var useCase = new GetLibraryUseCase(
                ReadOnlyLibraryRepositoryBuilder.Build()
            );

            // Act
            await useCase.Handle(query, CancellationToken.None);
        }
    }
}