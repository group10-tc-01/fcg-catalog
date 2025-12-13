using FCG.Catalog.WebApi.Middleware;
using System.Diagnostics.CodeAnalysis;
using FCG.Catalog.Infrastructure.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FCG.Catalog.WebApi.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class ApiBuilderExtensions
    {
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();

            using var dbContext = scope.ServiceProvider.GetRequiredService<FcgCatalogDbContext>();

            dbContext.Database.Migrate();
        }

        public static void UseCustomerExceptionHandler(this IApplicationBuilder app)
        {
            app.UseMiddleware<GlobalExceptionMiddleware>();
        }

        public static void UseGlobalCorrelationId(this IApplicationBuilder app)
        {
            app.UseMiddleware<GlobalCorrelationIdMiddleware>();
        }
    }
}
