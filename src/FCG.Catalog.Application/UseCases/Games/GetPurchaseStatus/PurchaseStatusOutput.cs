namespace FCG.Catalog.Application.UseCases.Games.GetPurchaseStatus;

public record PurchaseStatusOutput(
    Guid CorrelationId,
    string Status,
    string? Message = null
);