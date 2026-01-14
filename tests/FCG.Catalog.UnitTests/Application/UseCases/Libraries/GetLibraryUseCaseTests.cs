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
            CachingBuilder.Reset();
        }

        [Fact]
        public async Task Handle_ShouldReturnLibraryFromCache_WhenCacheExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var libraryId = Guid.NewGuid();
            var query = new GetLibraryByUserIdQuery(userId);

            var cachedResponse = new GetLibraryByUserIdResponse
            {
                LibraryId = libraryId,
                LibraryGames = new List<LibraryGameDto>
                {
                    new LibraryGameDto
                    {
                        GameId = Guid.NewGuid(),
                        Title = "Game 1",
                        Description = "Description 1",
                        PurchasePrice = 50.00m,
                        PurchaseDate = DateTimeOffset.UtcNow
                    }
                }
            };

            var cacheKey = $"Endpoint:Library - User:{userId}";
            var cachedValue = JsonSerializer.Serialize(cachedResponse);

            CachingBuilder.SetupGetAsync(cacheKey, cachedValue);

            var useCase = new GetLibraryUseCase(
                ReadOnlyLibraryRepositoryBuilder.Build(),
                CachingBuilder.Build()
            );

            // Act
            var result = await useCase.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.LibraryId.Should().Be(libraryId);
            result.LibraryGames.Should().NotBeNull();
            result.LibraryGames.Should().HaveCount(1);
            result.LibraryGames![0].Title.Should().Be("Game 1");

            CachingBuilder.VerifyGetAsyncWasCalled(cacheKey, Times.Once());
            ReadOnlyLibraryRepositoryBuilder.VerifyGetByUserIdWithGamesAsyncWasNeverCalled();
        }

        [Fact]
        public async Task Handle_ShouldReturnLibraryFromDatabase_WhenCacheDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var gameId = Guid.NewGuid();
            var query = new GetLibraryByUserIdQuery(userId);

            var game = _gameBuilder.BuildWithId(gameId, price: 60.00m);
            var library = _libraryBuilder.BuildWithGame(userId, gameId, 60.00m);

            var cacheKey = $"Endpoint:Library - User:{userId}";

            CachingBuilder.SetupGetAsync(cacheKey, null);
            ReadOnlyLibraryRepositoryBuilder.SetupGetByUserIdWithGamesAsync(userId, library);
            CachingBuilder.SetupSetAsync();

            // Mock da navegação LibraryGame.Game
            var libraryGames = library.LibraryGames.ToList();
            if (libraryGames.Any())
            {
                var gameProperty = typeof(LibraryGame).GetProperty("Game");
                gameProperty?.SetValue(libraryGames[0], game);
            }

            var useCase = new GetLibraryUseCase(
                ReadOnlyLibraryRepositoryBuilder.Build(),
                CachingBuilder.Build()
            );

            // Act
            var result = await useCase.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.LibraryGames.Should().NotBeNull();
            result.LibraryGames.Should().HaveCount(1);
            result.LibraryGames![0].GameId.Should().Be(gameId);
            result.LibraryGames[0].Title.Should().Be(game.Title.Value);
            result.LibraryGames[0].PurchasePrice.Should().Be(60.00m);

            CachingBuilder.VerifyGetAsyncWasCalled(cacheKey, Times.Once());
            ReadOnlyLibraryRepositoryBuilder.VerifyGetByUserIdWithGamesAsyncWasCalled(userId, Times.Once());
            CachingBuilder.VerifySetAsyncWasCalled(cacheKey, Times.Once());
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyResponse_WhenLibraryDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var query = new GetLibraryByUserIdQuery(userId);
            var cacheKey = $"Endpoint:Library - User:{userId}";

            CachingBuilder.SetupGetAsync(cacheKey, null);
            ReadOnlyLibraryRepositoryBuilder.SetupGetByUserIdWithGamesAsync(userId, null);

            var useCase = new GetLibraryUseCase(
                ReadOnlyLibraryRepositoryBuilder.Build(),
                CachingBuilder.Build()
            );

            // Act
            var result = await useCase.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.LibraryId.Should().Be(Guid.Empty);
            result.LibraryGames.Should().NotBeNull();
            result.LibraryGames.Should().BeEmpty();

            CachingBuilder.VerifyGetAsyncWasCalled(cacheKey, Times.Once());
            ReadOnlyLibraryRepositoryBuilder.VerifyGetByUserIdWithGamesAsyncWasCalled(userId, Times.Once());
            CachingBuilder.VerifySetAsyncWasNeverCalled();
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyLibraryGames_WhenLibraryHasNoGames()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var libraryId = Guid.NewGuid();
            var query = new GetLibraryByUserIdQuery(userId);

            var library = _libraryBuilder.BuildWithId(libraryId, userId);
            var cacheKey = $"Endpoint:Library - User:{userId}";

            CachingBuilder.SetupGetAsync(cacheKey, null);
            ReadOnlyLibraryRepositoryBuilder.SetupGetByUserIdWithGamesAsync(userId, library);
            CachingBuilder.SetupSetAsync();

            var useCase = new GetLibraryUseCase(
                ReadOnlyLibraryRepositoryBuilder.Build(),
                CachingBuilder.Build()
            );

            // Act
            var result = await useCase.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.LibraryId.Should().Be(libraryId);
            result.LibraryGames.Should().NotBeNull();
            result.LibraryGames.Should().BeEmpty();

            CachingBuilder.VerifySetAsyncWasCalled(cacheKey, Times.Once());
        }

        [Fact]
        public async Task Handle_ShouldOrderGamesByPurchaseDateDescending_WhenLibraryHasMultipleGames()
        {
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

            var cacheKey = $"Endpoint:Library - User:{userId}";

            CachingBuilder.SetupGetAsync(cacheKey, null);
            ReadOnlyLibraryRepositoryBuilder.SetupGetByUserIdWithGamesAsync(userId, library);
            CachingBuilder.SetupSetAsync();

            var useCase = new GetLibraryUseCase(
                ReadOnlyLibraryRepositoryBuilder.Build(),
                CachingBuilder.Build()
            );

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
            var cacheKey = $"Endpoint:Library - User:{userId}";

            CachingBuilder.SetupGetAsync(cacheKey, null);
            ReadOnlyLibraryRepositoryBuilder.SetupGetByUserIdWithGamesAsync(userId, library);
            CachingBuilder.SetupSetAsync();

            var useCase = new GetLibraryUseCase(
                ReadOnlyLibraryRepositoryBuilder.Build(),
                CachingBuilder.Build()
            );

            // Act
            await useCase.Handle(query, CancellationToken.None);

            // Assert
            CachingBuilder.VerifySetAsyncWasCalled(cacheKey, Times.Once());
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

            var cacheKey = $"Endpoint:Library - User:{userId}";

            CachingBuilder.SetupGetAsync(cacheKey, null);
            ReadOnlyLibraryRepositoryBuilder.SetupGetByUserIdWithGamesAsync(userId, library);
            CachingBuilder.SetupSetAsync();

            var useCase = new GetLibraryUseCase(
                ReadOnlyLibraryRepositoryBuilder.Build(),
                CachingBuilder.Build()
            );

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
            var cacheKey = $"Endpoint:Library - User:{userId}";

            CachingBuilder.SetupGetAsync(cacheKey, null);
            ReadOnlyLibraryRepositoryBuilder.SetupGetByUserIdWithGamesAsync(userId, null);

            var useCase = new GetLibraryUseCase(
                ReadOnlyLibraryRepositoryBuilder.Build(),
                CachingBuilder.Build()
            );

            // Act
            var result = await useCase.Handle(query, CancellationToken.None);

            // Assert
            result.LibraryId.Should().Be(Guid.Empty);
            result.LibraryGames.Should().BeEmpty();
            CachingBuilder.VerifySetAsyncWasNeverCalled();
        }

        [Fact]
        public async Task Handle_ShouldUseCorrectCacheKey_ForSpecificUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var query = new GetLibraryByUserIdQuery(userId);
            var expectedCacheKey = $"Endpoint:Library - User:{userId}";

            var library = _libraryBuilder.BuildWithUserId(userId);

            CachingBuilder.SetupGetAsync(expectedCacheKey, null);
            ReadOnlyLibraryRepositoryBuilder.SetupGetByUserIdWithGamesAsync(userId, library);
            CachingBuilder.SetupSetAsync();

            var useCase = new GetLibraryUseCase(
                ReadOnlyLibraryRepositoryBuilder.Build(),
                CachingBuilder.Build()
            );

            // Act
            await useCase.Handle(query, CancellationToken.None);

            // Assert
            CachingBuilder.VerifyGetAsyncWasCalled(expectedCacheKey, Times.Once());
        }

        [Fact]
        public async Task Handle_ShouldDeserializeCachedDataCorrectly_WhenCacheHit()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var libraryId = Guid.NewGuid();
            var gameId = Guid.NewGuid();
            var query = new GetLibraryByUserIdQuery(userId);

            var cachedResponse = new GetLibraryByUserIdResponse
            {
                LibraryId = libraryId,
                LibraryGames = new List<LibraryGameDto>
                {
                    new LibraryGameDto
                    {
                        GameId = gameId,
                        Title = "Cached Game",
                        Description = "Cached Description",
                        PurchasePrice = 99.99m,
                        PurchaseDate = DateTimeOffset.UtcNow.AddDays(-7)
                    }
                }
            };

            var cacheKey = $"Endpoint:Library - User:{userId}";
            var cachedValue = JsonSerializer.Serialize(cachedResponse);

            CachingBuilder.SetupGetAsync(cacheKey, cachedValue);

            var useCase = new GetLibraryUseCase(
                ReadOnlyLibraryRepositoryBuilder.Build(),
                CachingBuilder.Build()
            );

            // Act
            var result = await useCase.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.LibraryId.Should().Be(libraryId);
            result.LibraryGames.Should().HaveCount(1);
            result.LibraryGames![0].GameId.Should().Be(gameId);
            result.LibraryGames[0].Title.Should().Be("Cached Game");
            result.LibraryGames[0].PurchasePrice.Should().Be(99.99m);
        }
    }
}