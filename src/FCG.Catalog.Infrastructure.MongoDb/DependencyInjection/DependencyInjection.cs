using FCG.Catalog.Domain.Exception;
using FCG.Catalog.Messages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System.Diagnostics.CodeAnalysis;

namespace FCG.Catalog.Infrastructure.MongoDb.DependencyInjection
{
    [ExcludeFromCodeCoverage]
    public static class DependencyInjection
    {
        public static IServiceCollection AddMongoDbInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.UseMongoDB(configuration);

            return services;
        }

        private static void UseMongoDB(this IServiceCollection services, IConfiguration configuration)
        {
            var mongoConnectionString = configuration.GetConnectionString("MongoConnection");

            if (string.IsNullOrEmpty(mongoConnectionString))
            {
                throw new DomainException(ResourceMessages.MongoDbConnectionNotConfigured);
            }

            // Registra IMongoClient como Singleton
            services.AddSingleton<IMongoClient>(new MongoClient(mongoConnectionString));

            // Registra IMongoDatabase como Scoped
            services.AddScoped(provider =>
            {
                var client = provider.GetRequiredService<IMongoClient>();
                return client.GetDatabase("fcg_catalog");
            });

            // Registra DbContext para Entity Framework Core
            services.AddDbContext<MongoDbContext>(options =>
            {
                options.UseMongoDB(mongoConnectionString, databaseName: "fcg_catalog");
            });
        }
    }
}