using System.Diagnostics.CodeAnalysis;
using FCG.Catalog.Application.Abstractions.Messaging;

namespace FCG.Catalog.Application.UseCases.Games.ProcessPurchase
{
    [ExcludeFromCodeCoverage]
    public record PurchaseGameInput(Guid Id) : ICommand<PurchaseGameOutput>;

}