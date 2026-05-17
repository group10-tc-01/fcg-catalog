using FCG.Catalog.Application.Abstractions.Caching;
using FCG.Catalog.Application.UseCases.Games.Get;
using FCG.Catalog.Application.UseCases.Games.GetById;
using FCG.Catalog.CommomTestUtilities.Builders.Games;
using FCG.Catalog.CommomTestUtilities.Builders.Games.Repositories;
using FCG.Catalog.Domain.Catalog.Entities.Games;
using FCG.Catalog.Domain.Enum;
using FCG.Catalog.Domain.Exception;
using FCG.Catalog.Domain.Models;
using FCG.Catalog.Domain.Repositories.Game;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace FCG.Catalog.UnitTests.Application.UseCases.Games
{
    public class GetGameUseCaseTests
    {
        private readonly GameBuilder _gameBuilder;

        public GetGameUseCaseTests()
        {
            _gameBuilder = new GameBuilder();
            ReadOnlyGameRepositoryBuilder.Reset();
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoGamesExist()
        {
            // Arrange
            var emptyList = new List<Game>().AsQueryable();
            var input = new GetGameInput
            {
                Pagination = new PaginationParams { PageNumber = 1, PageSize = 10 }
            };

            ReadOnlyGameRepositoryBuilder.SetupGetAllWithFilters(emptyList);

            var useCase = new GetGameUseCase(ReadOnlyGameRepositoryBuilder.Build());

            // Act
            var result = await useCase.Handle(input, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().BeEmpty();
            result.TotalCount.Should().Be(0);
        }

        [Fact]
        public async Task Handle_ShouldApplyPagination_WhenPaginationIsProvided()
        {
            // Arrange
            var allGames = _gameBuilder.BuildList(25).AsQueryable();

            var input = new GetGameInput
            {
                Pagination = new PaginationParams { PageNumber = 2, PageSize = 10 }
            };

            ReadOnlyGameRepositoryBuilder.SetupGetAllWithFilters(allGames);

            var useCase = new GetGameUseCase(ReadOnlyGameRepositoryBuilder.Build());

            // Act
            var result = await useCase.Handle(input, CancellationToken.None);

            // Assert
            result.CurrentPage.Should().Be(2);
            result.PageSize.Should().Be(10);
            result.TotalCount.Should().Be(25);
            result.Items.Should().HaveCount(10);
        }

        [Fact]
        public async Task Handle_ShouldMapGameProperties_Correctly()
        {
            // Arrange
            var game = _gameBuilder.BuildWithAllParameters(
                "Test Game",
                "Test Description",
                59.99m,
                GameCategory.RPG
            );
            var games = new List<Game> { game }.AsQueryable();

            var input = new GetGameInput
            {
                Pagination = new PaginationParams { PageNumber = 1, PageSize = 10 }
            };

            ReadOnlyGameRepositoryBuilder.SetupGetAllWithFilters(games);

            var useCase = new GetGameUseCase(ReadOnlyGameRepositoryBuilder.Build());

            // Act
            var result = await useCase.Handle(input, CancellationToken.None);

            // Assert
            var firstGame = result.Items.First();
            firstGame.Title.Should().Be("Test Game");
            firstGame.Description.Should().Be("Test Description");
            firstGame.Price.Should().Be(59.99m);
            firstGame.FinalPrice.Should().Be(59.99m);
            firstGame.Category.Should().Be(GameCategory.RPG.ToString());
        }

        [Fact]
        public async Task Handle_ShouldReturnCachedList_WhenCacheHit()
        {
            // Arrange
            var input = new GetGameInput
            {
                Pagination = new PaginationParams { PageNumber = 1, PageSize = 10 }
            };
            var pagination = input.Pagination!;
            var cached = new PagedListResponse<GetGameOutput>(
                new List<GetGameOutput>
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Title = "Cached Game",
                        Description = "Cached Description",
                        Price = 10m,
                        FinalPrice = 10m,
                        Category = GameCategory.Action.ToString()
                    }
                },
                totalCount: 1,
                currentPage: 1,
                pageSize: 10);

            var readRepoMock = new Mock<IReadOnlyGameRepository>(MockBehavior.Strict);
            var cacheMock = new Mock<IGameCacheService>();
            cacheMock
                .Setup(cache => cache.GetGameListAsync(input, pagination, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cached);

            var useCase = new GetGameUseCase(readRepoMock.Object, cacheMock.Object);

            // Act
            var result = await useCase.Handle(input, CancellationToken.None);

            // Assert
            result.Should().BeSameAs(cached);
            readRepoMock.VerifyNoOtherCalls();
            cacheMock.Verify(
                cache => cache.SetGameListAsync(
                    It.IsAny<GetGameInput>(),
                    It.IsAny<PaginationParams>(),
                    It.IsAny<PagedListResponse<GetGameOutput>>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldCacheList_WhenCacheMiss()
        {
            // Arrange
            var allGames = _gameBuilder.BuildList(3).AsQueryable();
            var input = new GetGameInput
            {
                Pagination = new PaginationParams { PageNumber = 1, PageSize = 10 }
            };
            var pagination = input.Pagination!;

            ReadOnlyGameRepositoryBuilder.SetupGetAllWithFilters(allGames);

            var cacheMock = new Mock<IGameCacheService>();
            cacheMock
                .Setup(cache => cache.GetGameListAsync(input, pagination, It.IsAny<CancellationToken>()))
                .ReturnsAsync((PagedListResponse<GetGameOutput>?)null);
            cacheMock
                .Setup(cache => cache.SetGameListAsync(
                    input,
                    pagination,
                    It.IsAny<PagedListResponse<GetGameOutput>>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var useCase = new GetGameUseCase(ReadOnlyGameRepositoryBuilder.Build(), cacheMock.Object);

            // Act
            var result = await useCase.Handle(input, CancellationToken.None);

            // Assert
            result.Items.Should().HaveCount(3);
            cacheMock.Verify(
                cache => cache.SetGameListAsync(
                    input,
                    pagination,
                    It.Is<PagedListResponse<GetGameOutput>>(response => response.TotalCount == 3),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }

    public class GetGameIdUseCaseTests
    {
        private readonly GameBuilder _gameBuilder = new();

        [Fact]
        public async Task Handle_ShouldReturnCachedDetail_WhenCacheHit()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var input = new GetGameIdInput(gameId);
            var cached = new GetGameIdOutput
            {
                Title = "Cached Game",
                Description = "Cached Description",
                Category = GameCategory.Action.ToString(),
                OriginalPrice = 10m,
                DiscountedPrice = null,
                HasActivePromotion = false
            };

            var readRepoMock = new Mock<IReadOnlyGameRepository>(MockBehavior.Strict);
            var cacheMock = new Mock<IGameCacheService>();
            cacheMock
                .Setup(cache => cache.GetGameByIdAsync(gameId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cached);

            var loggerMock = new Mock<ILogger<GetGameIdUseCase>>();

            var useCase = new GetGameIdUseCase(readRepoMock.Object, cacheMock.Object, loggerMock.Object);

            // Act
            var result = await useCase.Handle(input, CancellationToken.None);

            // Assert
            result.Should().BeSameAs(cached);
            readRepoMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Handle_ShouldCacheDetail_WhenCacheMiss()
        {
            // Arrange
            var game = _gameBuilder.BuildWithAllParameters(
                "Detail Game",
                "Detail Description",
                49.99m,
                GameCategory.RPG);
            var input = new GetGameIdInput(game.Id);

            var readRepoMock = new Mock<IReadOnlyGameRepository>();
            readRepoMock
                .Setup(repo => repo.GetByIdAsync(game.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(game);

            var cacheMock = new Mock<IGameCacheService>();
            cacheMock
                .Setup(cache => cache.GetGameByIdAsync(game.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetGameIdOutput?)null);
            cacheMock
                .Setup(cache => cache.SetGameByIdAsync(
                    game.Id,
                    It.IsAny<GetGameIdOutput>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var loggerMock = new Mock<ILogger<GetGameIdUseCase>>();

            var useCase = new GetGameIdUseCase(readRepoMock.Object, cacheMock.Object, loggerMock.Object);

            // Act
            var result = await useCase.Handle(input, CancellationToken.None);

            // Assert
            result.Title.Should().Be("Detail Game");
            cacheMock.Verify(
                cache => cache.SetGameByIdAsync(
                    game.Id,
                    It.Is<GetGameIdOutput>(output => output.Title == "Detail Game"),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenCacheMissAndGameDoesNotExist()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var input = new GetGameIdInput(gameId);

            var readRepoMock = new Mock<IReadOnlyGameRepository>();
            readRepoMock
                .Setup(repo => repo.GetByIdAsync(gameId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Game?)null);

            var cacheMock = new Mock<IGameCacheService>();
            cacheMock
                .Setup(cache => cache.GetGameByIdAsync(gameId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetGameIdOutput?)null);

            var loggerMock = new Mock<ILogger<GetGameIdUseCase>>();

            var useCase = new GetGameIdUseCase(readRepoMock.Object, cacheMock.Object, loggerMock.Object);

            // Act
            Func<Task> act = async () => await useCase.Handle(input, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
            cacheMock.Verify(
                cache => cache.SetGameByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<GetGameIdOutput>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}
