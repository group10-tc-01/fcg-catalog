using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FCG.Catalog.Infrastructure.Redis
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddRedisInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Redis");

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = connectionString;
                options.InstanceName = "FCG_Catalog_";
            });

            return services;
        }
    }
}