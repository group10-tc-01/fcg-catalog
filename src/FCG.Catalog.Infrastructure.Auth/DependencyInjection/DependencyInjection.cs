using FCG.Catalog.Domain.Services.Repositories;
using FCG.Catalog.Infrastructure.Auth.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace FCG.Catalog.Infrastructure.Auth.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAuthInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<ICatalogLoggedUser, CatalogLoggedUser>();

            return services;
        }
    }
}