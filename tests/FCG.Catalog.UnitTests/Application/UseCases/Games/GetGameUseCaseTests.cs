using FCG.Catalog.Application.UseCases.Games.Get;
using FCG.Catalog.CommomTestUtilities.Builders.Games;
using FCG.Catalog.CommomTestUtilities.Builders.Games.Repositories;
using FCG.Catalog.Domain.Catalog.Entities.Games;
using FCG.Catalog.Domain.Enum;
using FCG.Catalog.Domain.Models;
using FluentAssertions;

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
    }
}