using FCG.Catalog.Domain.Abstractions;
using FCG.Catalog.Domain.Catalog.Entities.Games;
using FCG.Catalog.Domain.Catalog.Entities.Libraries;
using FCG.Catalog.Domain.Catalog.Entities.LibraryGames;
using FCG.Catalog.Domain.Catalog.Entities.Promotions;
using FCG.Catalog.Infrastructure.SqlServer.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace FCG.Catalog.Infrastructure.SqlServer
{
    public class FcgCatalogDbContext : DbContext
    {
        private readonly AuditingInterceptor? _auditingInterceptor;

        public DbSet<Library> Libraries { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<LibraryGame> LibraryGames { get; set; }
        public DbSet<PurchaseTransaction> PurchaseTransactions { get; set; }
        public DbSet<AuditTrail> AuditTrail { get; set; }

        public FcgCatalogDbContext(DbContextOptions<FcgCatalogDbContext> options) : base(options) { }

        public FcgCatalogDbContext(DbContextOptions<FcgCatalogDbContext> options, AuditingInterceptor auditingInterceptor)
            : base(options)
        {
            _auditingInterceptor = auditingInterceptor;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_auditingInterceptor is not null)
                optionsBuilder.AddInterceptors(_auditingInterceptor);

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(FcgCatalogDbContext).Assembly);
        }
    }
}


