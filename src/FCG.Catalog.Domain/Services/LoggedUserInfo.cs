using System.Diagnostics.CodeAnalysis;

namespace FCG.Catalog.Domain.Services
{
    [ExcludeFromCodeCoverage]
    public sealed class LoggedUserInfo
    {
        public Guid Id { get; init; }
        public string? Role { get; init; }
        public string Email { get; init; } = string.Empty;
    }
}