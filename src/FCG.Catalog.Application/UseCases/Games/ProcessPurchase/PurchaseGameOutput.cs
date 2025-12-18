using System.Diagnostics.CodeAnalysis;

namespace FCG.Catalog.Application.UseCases.Games.Purchase
{
    [ExcludeFromCodeCoverage]
    public record PurchaseGameOutput(string GameName, decimal OriginalPrice, decimal FinalPrice);
}