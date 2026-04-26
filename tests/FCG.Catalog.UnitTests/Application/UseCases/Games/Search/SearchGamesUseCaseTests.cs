using FCG.Catalog.Application.UseCases.Games.Search;
using FCG.Catalog.Domain.Models;
using FCG.Catalog.Domain.Repositories.Game;
using FluentAssertions;
using Moq;

namespace FCG.Catalog.UnitTests.Application.UseCases.Games.Search
{
    public class SearchGamesUseCaseTests
    {
        [Fact]
        public async Task Handle_ShouldSearchGamesUsingElasticsearchRepository()
        {
            // Arrange
            var cancellationToken = new CancellationTokenSource().Token;
            var input = new SearchGamesInput
            {
                Term = "zelda",
                PageNumber = 2,
                PageSize = 2
            };

            var indexedAt = DateTime.UtcNow;
            var repositoryResult = new PagedListResponse<GameSearch>(
                new List<GameSearch>
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Title = "The Legend of Zelda",
                        Description = "Adventure game",
                        Price = 59.99m,
                        Category = "Adventure",
                        DiscountedPrice = 49.99m,
                        IsActive = true,
                        IndexedAt = indexedAt,
                        Score = 1.85
                    }
                },
                totalCount: 5,
                currentPage: 2,
                pageSize: 2);

            var repository = new Mock<IGameSearchRepository>();
            repository
                .Setup(x => x.SearchAsync(
                    input.Term,
                    It.Is<PaginationParams>(p => p.PageNumber == input.PageNumber && p.PageSize == input.PageSize),
                    cancellationToken))
                .ReturnsAsync(repositoryResult);

            var useCase = new SearchGamesUseCase(repository.Object);

            // Act
            var result = await useCase.Handle(input, cancellationToken);

            // Assert
            result.CurrentPage.Should().Be(2);
            result.PageSize.Should().Be(2);
            result.TotalCount.Should().Be(5);
            result.TotalPages.Should().Be(3);
            result.Items.Should().ContainSingle();
            result.Items[0].Title.Should().Be("The Legend of Zelda");
            result.Items[0].DiscountedPrice.Should().Be(49.99m);
            result.Items[0].IndexedAt.Should().Be(indexedAt);
            result.Items[0].Score.Should().Be(1.85);

            repository.VerifyAll();
        }
    }
}
