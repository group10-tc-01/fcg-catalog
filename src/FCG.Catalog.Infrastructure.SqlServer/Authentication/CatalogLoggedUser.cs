using System;
using System.Linq;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using FCG.Catalog.Domain.Services;
using FCG.Catalog.Domain.Services.Repositories;

namespace FCG.Catalog.Infrastructure.SqlServer.Services
{

    public class CatalogLoggedUser : ICatalogLoggedUser
    {
        private readonly ITokenProvider _tokenProvider;

        public CatalogLoggedUser(ITokenProvider tokenProvider)
        {
            _tokenProvider = tokenProvider;
        }

        public Task<LoggedUserInfo?> GetLoggedUserAsync()
        {
            var token = _tokenProvider.GetToken();
            if (string.IsNullOrWhiteSpace(token))
                return Task.FromResult<LoggedUserInfo?>(null);

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            var idClaim = jwt.Claims.FirstOrDefault(c => c.Type == "nameid" || c.Type == "sub");
            if (idClaim == null || !Guid.TryParse(idClaim.Value, out var userId))
                return Task.FromResult<LoggedUserInfo?>(null);

            var role = jwt.Claims.FirstOrDefault(c => c.Type == "role")?.Value;

            return Task.FromResult<LoggedUserInfo?>(new LoggedUserInfo { Id = userId, Role = role });
        }
    }
}
