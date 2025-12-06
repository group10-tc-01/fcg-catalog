using FCG.Catalog.Domain;
using FCG.Catalog.Domain.Repositories.Game;
using FCG.Catalog.Infrastructure.SqlServer.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FCG.Catalog.Infrastructure.SqlServer.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSqlServer(configuration);

            services.AddScoped<IWriteOnlyGameRepository, GameRepository>();
            services.AddScoped<IReadOnlyGameRepository, GameRepository>();

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
