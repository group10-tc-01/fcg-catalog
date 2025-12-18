using System;
using FCG.Catalog.Domain.Services;
using FCG.Catalog.Domain.Services.Repositories;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FCG.Catalog.Infrastructure.SqlServer.Services
{
    [ExcludeFromCodeCoverage]
    public class CatalogLoggedUser : ICatalogLoggedUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CatalogLoggedUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<LoggedUserInfo?> GetLoggedUserAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null || !user.Identity.IsAuthenticated)
                return Task.FromResult<LoggedUserInfo?>(null);

            var idClaim = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                          ?? user.FindFirst("sub")?.Value;

            var role = user.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            if (Guid.TryParse(idClaim, out var userId))
            {
                return Task.FromResult<LoggedUserInfo?>(new LoggedUserInfo { Id = userId, Role = role });
            }

            return Task.FromResult<LoggedUserInfo?>(null);
        }
    }
}