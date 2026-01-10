namespace FCG.Catalog.Application.UseCases.Games.ProcessPurchase.GetPurchaseStatus;

public record PurchaseStatusOutput(
    Guid CorrelationId, 
    string Status, 
    string? Message = null
);