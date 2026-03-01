using FCG.Catalog.Application.DependencyInjection;
using FCG.Catalog.Infrastructure.Kafka.DependencyInjection;
using FCG.Catalog.WebApi.DependencyInjection;
using FCG.Catalog.WebApi.Extensions;
using FCG.Catalog.WebApi.Middleware;
using System.Text.Json.Serialization;
using Serilog;

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
            builder.Services.AddWebApi(builder.Configuration, builder.Host);
            builder.Services.AddSwaggerGen();

            builder.Services.AddKafkaInfrastructure(builder.Configuration);

            var app = builder.Build();

            app.UseElmahIo();
            app.UseMiddleware<GlobalExceptionMiddleware>();
            app.UseSerilogRequestLogging();

            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Application started successfully");
            logger.LogInformation("Environment: {Environment}", app.Environment.EnvironmentName);

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}

