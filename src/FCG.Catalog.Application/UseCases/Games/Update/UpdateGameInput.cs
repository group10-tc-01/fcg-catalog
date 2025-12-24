using FCG.Catalog.Application.Abstractions.Messaging;
using FCG.Catalog.Domain.Enum;
using MediatR;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace FCG.Catalog.Application.UseCases.Games.Update
{
    [ExcludeFromCodeCoverage]
    public class UpdateGameInput : IRequest<UpdateGameOutput>
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        public string Title { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public GameCategory Category { get; set; }
        public decimal Price { get; init; }
    }
}