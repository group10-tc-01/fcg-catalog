using FCG.Catalog.Application.Abstractions.Messaging;
using System;
using System.Diagnostics.CodeAnalysis;

namespace FCG.Catalog.Application.UseCases.Games.Purchase
{
    [ExcludeFromCodeCoverage]

    public record PurchaseGameInput(Guid Id, Guid UserId) : ICommand<PurchaseGameOutput>;

}