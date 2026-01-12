using FCG.Catalog.Domain.Abstractions;
using FCG.Catalog.Domain.Repositories.Game;
using FCG.Catalog.Domain.Repositories.Library;
using FCG.Catalog.Domain.Repositories.LibraryGame;
using FCG.Catalog.Domain.Repositories.Promotion;
using FCG.Catalog.Infrastructure.SqlServer.Repositories;
using FCG.Domain.Repositories.LibraryRepository;
using FCG.Domain.Repositories.PromotionRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace FCG.Catalog.Infrastructure.SqlServer.DependencyInjection
{
    [ExcludeFromCodeCoverage]
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSqlServer(configuration);

            services.AddScoped<IWriteOnlyGameRepository, GameRepository>();
            services.AddScoped<IReadOnlyGameRepository, GameRepository>();
            services.AddScoped<IReadOnlyPromotionRepository, PromotionRepository>();
            services.AddScoped<IWriteOnlyPromotionRepository, PromotionRepository>();
            services.AddScoped<IReadOnlyLibraryRepository, LibraryRepository>();
            services.AddScoped<IWriteOnlyLibraryRepository, LibraryRepository>();
            services.AddScoped<IReadOnlyLibraryGameRepository, LibraryGameRepository>();
            services.AddScoped<IWriteOnlyLibraryGameRepository, LibraryGameRepository>();
            services.AddScoped<IReadOnlyPurchaseTransactionRepository, PurchaseTransactionRepository>();
            services.AddScoped<IWriteOnlyPurchaseTransactionRepository, PurchaseTransactionRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }

        private static void AddSqlServer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<FcgCatalogDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });
        }


    }
}
