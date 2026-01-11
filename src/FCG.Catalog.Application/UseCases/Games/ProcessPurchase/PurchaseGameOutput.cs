using System.Diagnostics.CodeAnalysis;

namespace FCG.Catalog.Application.UseCases.Games.ProcessPurchase
{
    [ExcludeFromCodeCoverage]
    public record PurchaseGameOutput(
        Guid TransactionId,
        string Status,
        string Title,
        decimal FinalPrice
    );}