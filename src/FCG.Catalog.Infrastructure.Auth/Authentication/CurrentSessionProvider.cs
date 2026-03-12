using FCG.Catalog.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace FCG.Catalog.Infrastructure.Auth.Authentication;

public sealed class CurrentSessionProvider : ICurrentSessionProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string CorrelationIdKey = "CorrelationId";

    public CurrentSessionProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? GetUserId()
    {
        var user = _httpContextAccessor.HttpContext?.User;

        if (user == null || !user.Identity.IsAuthenticated)
            return null;

        var idClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                      ?? user.FindFirst("sub")?.Value;

        if (Guid.TryParse(idClaim, out var userId))
            return userId;

        return null;
    }

    public Guid? GetCorrelationId()
    {
        if (_httpContextAccessor.HttpContext?.Items.TryGetValue(CorrelationIdKey, out var correlationId) == true
            && correlationId is string correlationIdString
            && Guid.TryParse(correlationIdString, out var correlationGuid))
        {
            return correlationGuid;
        }

        return null;
    }

    public string? GetUserName()
    {
        var user = _httpContextAccessor.HttpContext?.User;

        if (user == null || !user.Identity.IsAuthenticated)
            return null;

        return user.FindFirst(ClaimTypes.Name)?.Value
               ?? user.FindFirst("name")?.Value;
    }
}
