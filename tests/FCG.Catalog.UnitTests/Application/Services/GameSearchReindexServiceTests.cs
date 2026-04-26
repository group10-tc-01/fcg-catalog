using FCG.Catalog.Application.Services;
using FCG.Catalog.CommomTestUtilities.Builders.Games;
using FCG.Catalog.Domain.Models;
using FCG.Catalog.Domain.Repositories.Game;
using Moq;

namespace FCG.Catalog.UnitTests.Application.Services
{
    public class GameSearchReindexServiceTests
    {
        [Fact]
        public async Task ReindexAsync_ShouldIndexAllGamesFromSqlRepository()
        {
            // Arrange
            var games = new GameBuilder().BuildList(3).AsQueryable();
            var readRepository = new Mock<IReadOnlyGameRepository>();
            readRepository
                .Setup(x => x.GetAllWithFilters(null, null, null, null))
                .Returns(games);

            var searchRepository = new Mock<IGameSearchRepository>();

            var service = new GameSearchReindexService(readRepository.Object, searchRepository.Object);

            // Act
            await service.ReindexAsync(CancellationToken.None);

            // Assert
            searchRepository.Verify(
                x => x.IndexAsync(It.IsAny<GameSearch>(), It.IsAny<CancellationToken>()),
                Times.Exactly(3));
        }
    }
}
