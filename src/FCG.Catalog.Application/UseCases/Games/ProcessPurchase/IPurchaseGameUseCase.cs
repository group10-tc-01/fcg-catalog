using FCG.Catalog.Application.Abstractions.Messaging;

namespace FCG.Catalog.Application.UseCases.Games.Purchase
{
    public interface IPurchaseGameUseCase : ICommandHandler<PurchaseGameInput, PurchaseGameOutput>
    {
    }

    
}
