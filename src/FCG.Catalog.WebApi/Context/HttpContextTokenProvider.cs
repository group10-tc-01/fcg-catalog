using System;
using System.Diagnostics.CodeAnalysis;
using FCG.Catalog.Domain.Services.Repositories;
using Microsoft.AspNetCore.Http;

namespace FCG.Catalog.WebApi.Services
{
    [ExcludeFromCodeCoverage]
    public sealed class HttpContextTokenProvider : ITokenProvider
    {
        private readonly IHttpContextAccessor _accessor;

        public HttpContextTokenProvider(IHttpContextAccessor accessor) =>
            _accessor = accessor;

        public string GetToken()
        {
            var ctx = _accessor.HttpContext;
            if (ctx == null) return string.Empty;

            if (ctx.Request.Headers.TryGetValue("Authorization", out var values))
            {
                var header = values.ToString();
                const string bearer = "Bearer ";
                if (header.StartsWith(bearer, StringComparison.OrdinalIgnoreCase))
                    return header.Substring(bearer.Length).Trim();
            }

            if (ctx.Request.Cookies.TryGetValue("access_token", out var cookieToken))
                return cookieToken;

            if (ctx.Items.TryGetValue("access_token", out var itemToken) && itemToken is string s)
                return s;

            return string.Empty;
        }
    }

}
