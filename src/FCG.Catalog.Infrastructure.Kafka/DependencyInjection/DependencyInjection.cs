using FCG.Catalog.Infrastructure.Kafka.Abstractions;
using FCG.Catalog.Infrastructure.Kafka.Producers;
using FCG.Catalog.Infrastructure.Kafka.Services;
using FCG.Catalog.Infrastructure.Kafka.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace FCG.Catalog.Infrastructure.Kafka.DependencyInjection
{
    [ExcludeFromCodeCoverage]
    public static class DependencyInjection
    {
        public static IServiceCollection AddKafkaInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<KafkaSettings>(
                configuration.GetSection("KafkaSettings"));

            services.AddKafkaProducer(configuration);
            services.AddKafkaConsumers(configuration);
            services.AddKafkaEventHandlers();

            return services;
        }

        public static IServiceCollection AddKafkaProducer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IKafkaProducer, KafkaProducerBase>();
            services.AddSingleton<KafkaProducerService>();

            return services;
        }

        private static void AddKafkaEventHandlers(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });
        }

        private static void RegisterConsumers(IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            var consumerTypes = assembly.GetTypes()
                .Where(t => t.IsClass
                            && !t.IsAbstract
                            && typeof(IKafkaConsumer).IsAssignableFrom(t));

            foreach (var consumerType in consumerTypes)
            {
                services.AddSingleton(typeof(IKafkaConsumer), consumerType);
            }
        }

        public static IServiceCollection AddKafkaConsumers(this IServiceCollection services, IConfiguration configuration)
        {
            RegisterConsumers(services);
            services.AddHostedService<KafkaConsumerService>();
            return services;
        }
    }
}