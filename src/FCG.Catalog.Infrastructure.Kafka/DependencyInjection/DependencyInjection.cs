using Confluent.Kafka;
using FCG.Catalog.Infrastructure.Kafka.Abstractions;
using FCG.Catalog.Infrastructure.Kafka.Producers;
using FCG.Catalog.Infrastructure.Kafka.Services;
using FCG.Catalog.Infrastructure.Kafka.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace FCG.Catalog.Infrastructure.Kafka.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddKafkaInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<KafkaSettings>(
                configuration.GetSection("KafkaSettings"));

            services.AddKafkaProducer(configuration);
            services.AddKafkaConsumers(configuration);

            return services;
        }

        public static IServiceCollection AddKafkaProducer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IKafkaProducer, KafkaProducerBase>();

            return services;
        }

        private static void RegisterConsumers(IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var consumerTypes = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(IKafkaConsumer).IsAssignableFrom(t))
                .ToList();

            foreach (var consumerType in consumerTypes)
            {
                services.AddScoped(consumerType);
                services.AddScoped<IKafkaConsumer>(sp =>
                    (IKafkaConsumer)sp.GetRequiredService(consumerType));
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