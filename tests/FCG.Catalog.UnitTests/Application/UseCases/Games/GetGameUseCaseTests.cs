using FCG.Catalog.Application.UseCases.Games.Get;
using FCG.Catalog.CommomTestUtilities.Builders.Games;
using FCG.Catalog.CommomTestUtilities.Builders.Games.Repositories;
using FCG.Catalog.Domain.Catalog.Entities.Games;
using FCG.Catalog.Domain.Enum;
using FCG.Catalog.Domain.Models;
using FluentAssertions;
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
        public async Task Handle_ShouldReturnPagedGames_WhenGamesExist()
        {
            // Arrange
            var games = _gameBuilder.BuildList(5);
            var input = new GetGameInput
            {
                Pagination = new PaginationParams { PageNumber = 1, PageSize = 10 }
            };

            ReadOnlyGameRepositoryBuilder.SetupGetAllWithFilters(games.AsQueryable());

            var useCase = new GetGameUseCase(ReadOnlyGameRepositoryBuilder.Build());

            // Act
            var result = await useCase.Handle(input, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(5);
            result.TotalCount.Should().Be(5);
            result.CurrentPage.Should().Be(1);
            result.PageSize.Should().Be(10);
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
        public async Task Handle_ShouldFilterByName_WhenNameIsProvided()
        {
            // Arrange
            var game1 = _gameBuilder.BuildWithName("Call of Duty");
            var game2 = _gameBuilder.BuildWithName("Battlefield");
            var games = new List<Game> { game1 }.AsQueryable();

            var input = new GetGameInput
            {
                Name = "Call of Duty",
                Pagination = new PaginationParams { PageNumber = 1, PageSize = 10 }
            };

            ReadOnlyGameRepositoryBuilder.SetupGetAllWithFilters(games, name: "Call of Duty");

            var useCase = new GetGameUseCase(ReadOnlyGameRepositoryBuilder.Build());

            // Act
            var result = await useCase.Handle(input, CancellationToken.None);

            // Assert
            result.Items.Should().HaveCount(1);
            result.Items.First().Title.Should().Be("Call of Duty");
            ReadOnlyGameRepositoryBuilder.VerifyGetAllWithFiltersWasCalled(Times.Once(), name: "Call of Duty");
        }

        [Fact]
        public async Task Handle_ShouldFilterByCategory_WhenCategoryIsProvided()
        {
            // Arrange
            var games = new List<Game>
            {
                _gameBuilder.BuildWithCategory(GameCategory.Action),
                _gameBuilder.BuildWithCategory(GameCategory.Action)
            }.AsQueryable();

            var input = new GetGameInput
            {
                Category = GameCategory.Action,
                Pagination = new PaginationParams { PageNumber = 1, PageSize = 10 }
            };

            ReadOnlyGameRepositoryBuilder.SetupGetAllWithFilters(games, category: GameCategory.Action);

            var useCase = new GetGameUseCase(ReadOnlyGameRepositoryBuilder.Build());

            // Act
            var result = await useCase.Handle(input, CancellationToken.None);

            // Assert
            result.Items.Should().HaveCount(2);
            result.Items.Should().OnlyContain(x => x.Category == GameCategory.Action.ToString());
        }

        [Fact]
        public async Task Handle_ShouldFilterByPriceRange_WhenMinAndMaxPriceAreProvided()
        {
            // Arrange
            var games = new List<Game>
            {
                _gameBuilder.BuildWithPrice(50),
                _gameBuilder.BuildWithPrice(75)
            }.AsQueryable();

            var input = new GetGameInput
            {
                MinPrice = 40,
                MaxPrice = 80,
                Pagination = new PaginationParams { PageNumber = 1, PageSize = 10 }
            };

            ReadOnlyGameRepositoryBuilder.SetupGetAllWithFilters(games, minPrice: 40, maxPrice: 80);

            var useCase = new GetGameUseCase(ReadOnlyGameRepositoryBuilder.Build());

            // Act
            var result = await useCase.Handle(input, CancellationToken.None);

            // Assert
            result.Items.Should().HaveCount(2);
            result.Items.Should().OnlyContain(x => x.Price >= 40 && x.Price <= 80);
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
        public async Task Handle_ShouldUseDefaultPagination_WhenPaginationIsNull()
        {
            // Arrange
            var games = _gameBuilder.BuildList(5);
            var input = new GetGameInput
            {
                Pagination = null
            };

            ReadOnlyGameRepositoryBuilder.SetupGetAllWithFilters(games.AsQueryable());

            var useCase = new GetGameUseCase(ReadOnlyGameRepositoryBuilder.Build());

            // Act
            var result = await useCase.Handle(input, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(5);
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
        public async Task Handle_ShouldCallRepository_WithCorrectFilters()
        {
            // Arrange
            var games = _gameBuilder.BuildList(3);
            var input = new GetGameInput
            {
                Name = "Test",
                Category = GameCategory.Action,
                MinPrice = 10,
                MaxPrice = 100,
                Pagination = new PaginationParams { PageNumber = 1, PageSize = 10 }
            };

            ReadOnlyGameRepositoryBuilder.SetupGetAllWithFilters(
                games.AsQueryable(),
                name: "Test",
                category: GameCategory.Action,
                minPrice: 10,
                maxPrice: 100
            );

            var useCase = new GetGameUseCase(ReadOnlyGameRepositoryBuilder.Build());

            // Act
            await useCase.Handle(input, CancellationToken.None);

            // Assert
            ReadOnlyGameRepositoryBuilder.VerifyGetAllWithFiltersWasCalled(
                Times.Once(),
                name: "Test",
                category: GameCategory.Action,
                minPrice: 10,
                maxPrice: 100
            );
        }
    }
}