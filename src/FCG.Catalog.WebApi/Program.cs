using FCG.Catalog.Infrastructure.Kafka.DependencyInjection;
using FCG.Catalog.Application.DependencyInjection;
using FCG.Catalog.WebApi.DependencyInjection;
using FCG.Catalog.WebApi.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
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
