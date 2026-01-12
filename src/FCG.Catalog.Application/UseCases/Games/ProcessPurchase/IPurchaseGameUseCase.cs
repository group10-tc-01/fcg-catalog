using FCG.Catalog.Application.Abstractions.Messaging;

namespace FCG.Catalog.Application.UseCases.Games.ProcessPurchase
{
    public interface IPurchaseGameUseCase : ICommandHandler<PurchaseGameInput, PurchaseGameOutput>
    {
    }


}
