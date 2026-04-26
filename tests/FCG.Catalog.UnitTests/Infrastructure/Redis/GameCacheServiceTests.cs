using FCG.Catalog.Application.UseCases.Games.Get;
using FCG.Catalog.Application.UseCases.Games.GetById;
using FCG.Catalog.Domain.Enum;
using FCG.Catalog.Domain.Models;
using FCG.Catalog.Infrastructure.Redis.Services;
using FCG.Catalog.Infrastructure.Redis.Settings;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace FCG.Catalog.UnitTests.Infrastructure.Redis
{
    public class GameCacheServiceTests
    {
        [Fact]
        public async Task SetAndGetGameByIdAsync_ShouldSerializeAndDeserializeDetail()
        {
            // Arrange
            using var provider = CreateProvider();
            var service = CreateService(provider.GetRequiredService<IDistributedCache>());
            var gameId = Guid.NewGuid();
            var expected = new GetGameIdOutput
            {
                Title = "Cached Detail",
                Description = "Cached Description",
                Category = GameCategory.Action.ToString(),
                OriginalPrice = 39.99m,
                DiscountedPrice = 29.99m,
                HasActivePromotion = true
            };

            // Act
            await service.SetGameByIdAsync(gameId, expected, CancellationToken.None);
            var result = await service.GetGameByIdAsync(gameId, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result!.Title.Should().Be(expected.Title);
            result.Description.Should().Be(expected.Description);
            result.Category.Should().Be(expected.Category);
            result.OriginalPrice.Should().Be(expected.OriginalPrice);
            result.DiscountedPrice.Should().Be(expected.DiscountedPrice);
            result.HasActivePromotion.Should().Be(expected.HasActivePromotion);
        }

        [Fact]
        public async Task SetAndGetGameListAsync_ShouldSerializeAndDeserializeList()
        {
            // Arrange
            using var provider = CreateProvider();
            var service = CreateService(provider.GetRequiredService<IDistributedCache>());
            var input = new GetGameInput
            {
                Name = "zelda",
                Category = GameCategory.Adventure,
                MinPrice = 40m,
                MaxPrice = 60m,
                Pagination = new PaginationParams { PageNumber = 2, PageSize = 5 }
            };
            var pagination = input.Pagination;
            var expected = new PagedListResponse<GetGameOutput>(
                new List<GetGameOutput>
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Title = "The Legend of Zelda",
                        Description = "Adventure game",
                        Price = 59.99m,
                        FinalPrice = 49.99m,
                        Category = GameCategory.Adventure.ToString()
                    }
                },
                totalCount: 6,
                currentPage: 2,
                pageSize: 5);

            // Act
            await service.SetGameListAsync(input, pagination, expected, CancellationToken.None);
            var result = await service.GetGameListAsync(input, pagination, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result!.CurrentPage.Should().Be(2);
            result.PageSize.Should().Be(5);
            result.TotalCount.Should().Be(6);
            result.Items.Should().ContainSingle();
            result.Items[0].Title.Should().Be("The Legend of Zelda");
        }

        [Fact]
        public async Task InvalidateGameListAsync_ShouldRotateVersionAndHidePreviousListEntries()
        {
            // Arrange
            using var provider = CreateProvider();
            var service = CreateService(provider.GetRequiredService<IDistributedCache>());
            var input = new GetGameInput
            {
                Pagination = new PaginationParams { PageNumber = 1, PageSize = 10 }
            };
            var pagination = input.Pagination;
            var expected = new PagedListResponse<GetGameOutput>(
                new List<GetGameOutput>
                {
                    new() { Id = Guid.NewGuid(), Title = "Cached Game" }
                },
                totalCount: 1,
                currentPage: 1,
                pageSize: 10);

            await service.SetGameListAsync(input, pagination, expected, CancellationToken.None);

            // Act
            await service.InvalidateGameListAsync(CancellationToken.None);
            var result = await service.GetGameListAsync(input, pagination, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetGameListAsync_ShouldReturnNull_WhenCacheIsEmpty()
        {
            // Arrange
            using var provider = CreateProvider();
            var service = CreateService(provider.GetRequiredService<IDistributedCache>());
            var input = new GetGameInput
            {
                Pagination = new PaginationParams { PageNumber = 1, PageSize = 10 }
            };

            // Act
            var result = await service.GetGameListAsync(input, input.Pagination, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetGameByIdAsync_ShouldReturnNull_WhenCacheIsEmpty()
        {
            // Arrange
            using var provider = CreateProvider();
            var service = CreateService(provider.GetRequiredService<IDistributedCache>());

            // Act
            var result = await service.GetGameByIdAsync(Guid.NewGuid(), CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task InvalidateGameByIdAsync_ShouldRemoveCachedDetail()
        {
            // Arrange
            using var provider = CreateProvider();
            var service = CreateService(provider.GetRequiredService<IDistributedCache>());
            var gameId = Guid.NewGuid();
            var expected = new GetGameIdOutput
            {
                Title = "Cached Detail",
                Description = "Cached Description",
                Category = GameCategory.Action.ToString(),
                OriginalPrice = 39.99m,
                HasActivePromotion = false
            };

            await service.SetGameByIdAsync(gameId, expected, CancellationToken.None);

            // Act
            await service.InvalidateGameByIdAsync(gameId, CancellationToken.None);
            var result = await service.GetGameByIdAsync(gameId, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task CacheOperations_ShouldFallback_WhenDistributedCacheThrows()
        {
            // Arrange
            var service = CreateService(new ThrowingDistributedCache());
            var gameId = Guid.NewGuid();
            var input = new GetGameInput
            {
                Name = "cache",
                Category = GameCategory.Action,
                MinPrice = 10m,
                MaxPrice = 20m,
                Pagination = new PaginationParams { PageNumber = 1, PageSize = 10 }
            };
            var listResponse = new PagedListResponse<GetGameOutput>(
                new List<GetGameOutput> { new() { Id = gameId, Title = "Cache" } },
                totalCount: 1,
                currentPage: 1,
                pageSize: 10);
            var detailResponse = new GetGameIdOutput
            {
                Title = "Cache",
                Description = "Fallback",
                Category = GameCategory.Action.ToString(),
                OriginalPrice = 10m,
                HasActivePromotion = false
            };

            // Act
            var listResult = await service.GetGameListAsync(input, input.Pagination, CancellationToken.None);
            var detailResult = await service.GetGameByIdAsync(gameId, CancellationToken.None);
            var writeAct = async () =>
            {
                await service.SetGameListAsync(input, input.Pagination, listResponse, CancellationToken.None);
                await service.SetGameByIdAsync(gameId, detailResponse, CancellationToken.None);
                await service.InvalidateGameListAsync(CancellationToken.None);
                await service.InvalidateGameByIdAsync(gameId, CancellationToken.None);
            };

            // Assert
            listResult.Should().BeNull();
            detailResult.Should().BeNull();
            await writeAct.Should().NotThrowAsync();
        }

        private static ServiceProvider CreateProvider()
        {
            var services = new ServiceCollection();
            services.AddDistributedMemoryCache();
            return services.BuildServiceProvider();
        }

        private static GameCacheService CreateService(IDistributedCache distributedCache)
        {
            var settings = Options.Create(new RedisSettings
            {
                ConnectionString = "localhost:6379",
                InstanceName = "fcg-catalog:",
                DefaultTtlSeconds = 60
            });
            var logger = new Mock<ILogger<GameCacheService>>();

            return new GameCacheService(distributedCache, settings, logger.Object);
        }

        private sealed class ThrowingDistributedCache : IDistributedCache
        {
            public byte[]? Get(string key)
            {
                throw new InvalidOperationException("Cache unavailable.");
            }

            public Task<byte[]?> GetAsync(string key, CancellationToken token = default)
            {
                throw new InvalidOperationException("Cache unavailable.");
            }

            public void Refresh(string key)
            {
                throw new InvalidOperationException("Cache unavailable.");
            }

            public Task RefreshAsync(string key, CancellationToken token = default)
            {
                throw new InvalidOperationException("Cache unavailable.");
            }

            public void Remove(string key)
            {
                throw new InvalidOperationException("Cache unavailable.");
            }

            public Task RemoveAsync(string key, CancellationToken token = default)
            {
                throw new InvalidOperationException("Cache unavailable.");
            }

            public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
            {
                throw new InvalidOperationException("Cache unavailable.");
            }

            public Task SetAsync(
                string key,
                byte[] value,
                DistributedCacheEntryOptions options,
                CancellationToken token = default)
            {
                throw new InvalidOperationException("Cache unavailable.");
            }
        }
    }
}
