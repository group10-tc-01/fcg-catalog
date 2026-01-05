using FCG.Catalog.Domain.Catalog.Entities.LibraryGames;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using FCG.Catalog.Domain.Catalog.Entities.Games;
using FCG.Catalog.Domain.Catalog.Entities.Libraries;
using FCG.Catalog.Domain.Catalog.Entities.Promotions;

namespace FCG.Catalog.Infrastructure.SqlServer
{
    public class FcgCatalogDbContext : DbContext
    {
        public DbSet<Library> Libraries { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<LibraryGame> LibraryGames { get; set; }
        public FcgCatalogDbContext(DbContextOptions<FcgCatalogDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("Catalog");
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(FcgCatalogDbContext).Assembly);
        }

    }

}


