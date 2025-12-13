using System;

namespace FCG.Catalog.Domain.Services
{
    public sealed class LoggedUserInfo
    {
        public Guid Id { get; init; }
        public string? Role { get; init; }
    }
}