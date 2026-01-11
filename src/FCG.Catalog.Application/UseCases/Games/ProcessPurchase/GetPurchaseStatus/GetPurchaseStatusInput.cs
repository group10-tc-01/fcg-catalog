using MediatR;

namespace FCG.Catalog.Application.UseCases.Games.ProcessPurchase.GetPurchaseStatus;

public record GetPurchaseStatusInput(Guid CorrelationId) : IRequest<PurchaseStatusOutput>;