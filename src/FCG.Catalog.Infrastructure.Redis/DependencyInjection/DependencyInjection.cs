using System.Diagnostics.CodeAnalysis;
using FCG.Catalog.Application.Abstractions.Caching;
using FCG.Catalog.Infrastructure.Redis.Services;
using FCG.Catalog.Infrastructure.Redis.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FCG.Catalog.Infrastructure.Redis.DependencyInjection
{
    [ExcludeFromCodeCoverage]
    public static class DependencyInjection
    {
        public static IServiceCollection AddRedisInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration,
            IHostEnvironment environment)
        {
            var settingsSection = configuration.GetSection(RedisSettings.SectionName);
            var settings = settingsSection.Get<RedisSettings>() ?? new RedisSettings();

            services.Configure<RedisSettings>(settingsSection);

            if (environment.IsEnvironment("Test"))
            {
                services.AddDistributedMemoryCache();
            }
            else
            {
                if (string.IsNullOrWhiteSpace(settings.ConnectionString))
                {
                    throw new InvalidOperationException("RedisSettings:ConnectionString is required outside the Test environment.");
                }

                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = settings.ConnectionString;
                    options.InstanceName = settings.InstanceName;
                });
            }

            services.AddScoped<IGameCacheService, GameCacheService>();

            return services;
        }
    }
}
