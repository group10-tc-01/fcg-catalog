using FCG.Catalog.Application.Abstractions.Messaging;
using FCG.Catalog.Application.UseCases.Games.ProcessPurchase;

namespace FCG.Catalog.Application.UseCases.Games.Purchase
{
    public interface IPurchaseGameUseCase : ICommandHandler<PurchaseGameInput, PurchaseGameOutput>
    {
    }

    
}
