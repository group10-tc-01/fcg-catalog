using FCG.Catalog.CommomTestUtilities.Builders.Games;
using FCG.Catalog.CommomTestUtilities.Builders.Libraries;
using FCG.Catalog.CommomTestUtilities.Builders.Promotions;
using FCG.Catalog.Domain.Catalog.Entities.Games;
using FCG.Catalog.Domain.Catalog.Entities.Libraries;
using FCG.Catalog.Domain.Catalog.Entities.Promotions;
using FCG.Catalog.Infrastructure.Redis.Interface;
using FCG.Catalog.Infrastructure.SqlServer;
using FCG.Catalog.IntegratedTests.Fakes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using Program = FCG.Catalog.WebApi.Program;

namespace FCG.Catalog.IntegratedTests.Configurations
{
    [ExcludeFromCodeCoverage]
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        private DbConnection? _connection;
        public List<Library> CreatedLibraries { get; private set; } = [];
        public List<Game> CreatedGames { get; private set; } = [];
        public List<Promotion> CreatedPromotions { get; private set; } = [];

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Test").ConfigureServices(services =>
            {
                RemoveEntityFrameworkServices(services);
                RemoveKafkaServices(services);
                RemoveRedisServices(services);

                services.AddSingleton<ICaching, InMemoryCaching>();

                _connection?.Dispose();
                _connection = new SqliteConnection("Data Source=:memory:");
                _connection.Open();

                services.AddDbContext<FcgCatalogDbContext>(options =>
                {
                    options.UseSqlite(_connection)
                            .EnableSensitiveDataLogging()
                            .EnableDetailedErrors();
                });

                EnsureDatabaseSeeded(services);
            });
        }

        private static void RemoveEntityFrameworkServices(IServiceCollection services)
        {
            var descriptorsToRemove = services.Where(d =>
                d.ServiceType == typeof(DbContextOptions<FcgCatalogDbContext>) ||
                d.ServiceType == typeof(FcgCatalogDbContext) ||
                d.ServiceType.Namespace?.StartsWith("Microsoft.EntityFrameworkCore") == true)
                .ToList();

            foreach (var descriptor in descriptorsToRemove)
            {
                services.Remove(descriptor);
            }
        }

        private static void RemoveKafkaServices(IServiceCollection services)
        {
            var kafkaDescriptorsToRemove = services.Where(d =>
                d.ServiceType.FullName?.Contains("Kafka") == true ||
                d.ImplementationType?.FullName?.Contains("Kafka") == true)
                .ToList();

            foreach (var descriptor in kafkaDescriptorsToRemove)
            {
                services.Remove(descriptor);
            }
        }

        private static void RemoveRedisServices(IServiceCollection services)
        {
            var redisDescriptorsToRemove = services.Where(d =>
                d.ServiceType.FullName?.Contains("Redis") == true ||
                d.ServiceType.FullName?.Contains("Caching") == true ||
                d.ImplementationType?.FullName?.Contains("Redis") == true ||
                d.ImplementationType?.FullName?.Contains("Caching") == true)
                .ToList();

            foreach (var descriptor in redisDescriptorsToRemove)
            {
                services.Remove(descriptor);
            }
        }

        private void EnsureDatabaseSeeded(IServiceCollection services)
        {
            using var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<FcgCatalogDbContext>();


            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();

            StartDatabase(dbContext);
        }

        private void StartDatabase(FcgCatalogDbContext context)
        {
            var itemsQuantity = 3;


            CreatedLibraries = CreateLibraries(context, itemsQuantity);
            CreatedGames = CreateGames(context, itemsQuantity);
            CreatedPromotions = CreatePromotions(context, CreatedGames);
        }

        private List<Library> CreateLibraries(FcgCatalogDbContext context, int itemsQuantity)
        {
            var libraries = new List<Library>();

            for (int i = 1; i <= itemsQuantity; i++)
            {
                var library = new LibraryBuilder().BuildWithUserId(Guid.NewGuid());
                libraries.Add(library);
            }

            context.Libraries.AddRange(libraries);
            context.SaveChanges();

            return libraries;
        }

        private List<Game> CreateGames(FcgCatalogDbContext context, int itemsQuantity)
        {
            var games = new List<Game>();
            var gameBuilder = new GameBuilder();

            for (int i = 1; i <= itemsQuantity; i++)
            {
                var game = gameBuilder.Build();
                games.Add(game);
            }

            context.Games.AddRange(games);
            context.SaveChanges();

            return games;
        }

        private List<Promotion> CreatePromotions(FcgCatalogDbContext context, List<Game> games)
        {
            var promotions = new List<Promotion>();
            var promotionBuilder = new PromotionBuilder();

            foreach (var game in games.Take(2)) 
            {
                var promotion = promotionBuilder.BuildActivePromotion(game.Id, 20m);
                promotions.Add(promotion);
            }

            context.Promotions.AddRange(promotions);
            context.SaveChanges();

            return promotions;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _connection?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}