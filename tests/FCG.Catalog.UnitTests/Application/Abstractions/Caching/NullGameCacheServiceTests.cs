using FCG.Catalog.Application.Abstractions.Caching;
using FCG.Catalog.Application.UseCases.Games.Get;
using FCG.Catalog.Application.UseCases.Games.GetById;
using FCG.Catalog.Domain.Models;
using FluentAssertions;

namespace FCG.Catalog.UnitTests.Application.Abstractions.Caching
{
    public class NullGameCacheServiceTests
    {
        [Fact]
        public async Task GetMethods_ShouldReturnCacheMiss()
        {
            // Arrange
            var service = NullGameCacheService.Instance;
            var input = new GetGameInput();
            var pagination = new PaginationParams();

            // Act
            var listResult = await service.GetGameListAsync(input, pagination, CancellationToken.None);
            var detailResult = await service.GetGameByIdAsync(Guid.NewGuid(), CancellationToken.None);

            // Assert
            listResult.Should().BeNull();
            detailResult.Should().BeNull();
        }

        [Fact]
        public async Task WriteAndInvalidateMethods_ShouldCompleteWithoutSideEffects()
        {
            // Arrange
            var service = NullGameCacheService.Instance;
            var gameId = Guid.NewGuid();
            var input = new GetGameInput();
            var pagination = new PaginationParams();
            var listResponse = new PagedListResponse<GetGameOutput>(
                new List<GetGameOutput>(),
                totalCount: 0,
                currentPage: 1,
                pageSize: 10);
            var detailResponse = new GetGameIdOutput
            {
                Title = "Noop",
                Description = "Noop",
                Category = "Action",
                OriginalPrice = 10m,
                HasActivePromotion = false
            };

            // Act
            var act = async () =>
            {
                await service.SetGameListAsync(input, pagination, listResponse, CancellationToken.None);
                await service.SetGameByIdAsync(gameId, detailResponse, CancellationToken.None);
                await service.InvalidateGameListAsync(CancellationToken.None);
                await service.InvalidateGameByIdAsync(gameId, CancellationToken.None);
            };

            // Assert
            await act.Should().NotThrowAsync();
            NullGameCacheService.Instance.Should().BeSameAs(service);
        }
    }
}
