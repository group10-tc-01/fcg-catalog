using FCG.Catalog.Application.DependencyInjection;
using FCG.Catalog.Infrastructure.Kafka.DependencyInjection;
using FCG.Catalog.WebApi.DependencyInjection;
using FCG.Catalog.WebApi.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Esta opção faz o serializador ignorar a propriedade que causa o loop
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;

        // Opcional: Para formatar o JSON bonitinho
        options.JsonSerializerOptions.WriteIndented = true;
    }); builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApplication();
builder.Services.AddWebApi(builder.Configuration);
builder.Services.AddSwaggerGen();

builder.Services.AddKafkaInfrastructure(builder.Configuration); ;

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
