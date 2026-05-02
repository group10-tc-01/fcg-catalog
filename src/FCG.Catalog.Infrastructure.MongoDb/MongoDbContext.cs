using FCG.Catalog.Domain.Catalog.Entities.Games;
using Microsoft.EntityFrameworkCore;

namespace FCG.Catalog.Infrastructure.MongoDb
{
    public class MongoDbContext : DbContext
    {
        public DbSet<Game> Games { get; set; } = null!;

        public MongoDbContext(DbContextOptions<MongoDbContext> options) : base(options)
        {

        }
    }
}
