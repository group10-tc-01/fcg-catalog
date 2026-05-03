using FCG.Catalog.Domain.Catalog.Entities.Games;
using FCG.Catalog.Infrastructure.MongoDb.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace FCG.Catalog.Infrastructure.MongoDb
{
    [ExcludeFromCodeCoverage]
    public class MongoDbContext : DbContext
    {
        public DbSet<Game> Games { get; set; } = null!;
        public DbSet<GameDetailDocument> GamesDetail { get; set; } = null!;
        public DbSet<GameCacheEntity> GamesCache { get; set; } = null!;

        public MongoDbContext(DbContextOptions<MongoDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<GameDetailDocument>().ToCollection("games_detail");
            modelBuilder.Entity<GameCacheEntity>().ToCollection("games_cache");
        }
    }
}
