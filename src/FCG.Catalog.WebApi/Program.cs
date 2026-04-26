using FCG.Catalog.Application.DependencyInjection;
using FCG.Catalog.Infrastructure.Auth.DependencyInjection;
using FCG.Catalog.Infrastructure.Elasticsearch.DependencyInjection;
using FCG.Catalog.Infrastructure.Kafka.DependencyInjection;
using FCG.Catalog.Infrastructure.Redis.DependencyInjection;
using FCG.Catalog.Infrastructure.SqlServer.DependencyInjection;
using FCG.Catalog.WebApi.DependencyInjection;

namespace FCG.Catalog.WebApi
{
    public class Program
    {
        protected Program() { }
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddWebApi(builder.Configuration);

            builder.Services.AddApplication();

            builder.Services.AddAuthInfrastructure(builder.Configuration);

            builder.Services.AddElasticsearchInfrastructure(builder.Configuration);

            builder.Services.AddKafkaInfrastructure(builder.Configuration);

            builder.Services.AddRedisInfrastructure(builder.Configuration, builder.Environment);

            builder.Services.AddSqlServerInfrastructure(builder.Configuration);

            var app = builder.Build();

            app.UseWebApiPipeline();

            app.Run();
        }
    }
}

