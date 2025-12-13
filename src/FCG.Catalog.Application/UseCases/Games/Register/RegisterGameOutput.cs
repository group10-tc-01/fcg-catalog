using System;
using System.Diagnostics.CodeAnalysis;

namespace FCG.Catalog.Application.UseCases.Games.Register
{
    [ExcludeFromCodeCoverage]

    public class RegisterGameOutput
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
    }
}
