using FCG.Catalog.Domain.Catalog.Entities.Games;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace FCG.Catalog.Infrastructure.MongoDb
{
    [ExcludeFromCodeCoverage]
    public class MongoDbContext : DbContext
    {
        public DbSet<Game> Games { get; set; } = null!;

        public MongoDbContext(DbContextOptions<MongoDbContext> options) : base(options)
        {

        }
    }
}
