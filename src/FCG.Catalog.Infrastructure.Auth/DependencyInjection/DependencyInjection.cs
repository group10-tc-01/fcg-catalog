using FCG.Catalog.Domain.Interfaces;
using FCG.Catalog.Domain.Services.Repositories;
using FCG.Catalog.Infrastructure.Auth.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace FCG.Catalog.Infrastructure.Auth.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAuthInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<ICatalogLoggedUser, CatalogLoggedUser>();
            services.AddScoped<ICurrentSessionProvider, CurrentSessionProvider>();

            services.AddAuthenticationConfiguration(configuration);

            return services;
        }

        private static void AddAuthenticationConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];

            var key = Encoding.ASCII.GetBytes(secretKey!);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
              {
                  options.RequireHttpsMetadata = false;
                  options.SaveToken = true;
                  options.TokenValidationParameters = new TokenValidationParameters
                  {
                      ValidateIssuerSigningKey = true,
                      IssuerSigningKey = new SymmetricSecurityKey(key),

                      ValidateIssuer = true,
                      ValidIssuer = issuer,

                      ValidateAudience = true,
                      ValidAudience = audience,

                      ValidateLifetime = true,
                      ClockSkew = TimeSpan.Zero
                  };
              });
        }
    }
}