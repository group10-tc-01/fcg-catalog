using FCG.Catalog.Infrastructure.Redis.Interface;
using FCG.Catalog.Infrastructure.Redis.Redis;
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

            services.AddScoped<ICaching, Caching>();

            return services;
        }
    }
}