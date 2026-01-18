using FCG.Catalog.Application.DependencyInjection;
using FCG.Catalog.Infrastructure.Kafka.DependencyInjection;
using FCG.Catalog.Infrastructure.Redis.DependencyInjection;
using FCG.Catalog.Infrastructure.Redis.Redis;
using FCG.Catalog.WebApi.DependencyInjection;
using FCG.Catalog.WebApi.Extensions;
using FCG.Catalog.WebApi.Middleware;
using System.Text.Json.Serialization;
namespace FCG.Catalog.WebApi
{
    public class Program
    {
        protected Program() { }
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                    options.JsonSerializerOptions.WriteIndented = true;
                }); builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddApplication();
            builder.Services.AddWebApi(builder.Configuration);
            builder.Services.AddSwaggerGen();

            builder.Services.AddKafkaInfrastructure(builder.Configuration);
            builder.Services.AddRedisInfrastructure(builder.Configuration);
            builder.Services.Configure<CacheSettings>(builder.Configuration.GetSection("CacheSettings"));

            var app = builder.Build();

            app.UseMiddleware<GlobalExceptionMiddleware>();

            if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Docker")
            {
                app.ApplyMigrations();
            }

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}

