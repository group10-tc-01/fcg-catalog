using FCG.Catalog.Domain.Enum;
using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace FCG.Catalog.Application.UseCases.Games.Register
{
    [ExcludeFromCodeCoverage]
    public class RegisterGameInput : IRequest<RegisterGameOutput>
    {
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public decimal Price { get; init; }
        public GameCategory Category { get; init; }
    }
}
