using MediatR;

namespace FCG.Catalog.Application.UseCases.Games.GetPurchaseStatus;

public record GetPurchaseStatusInput(Guid CorrelationId) : IRequest<PurchaseStatusOutput>;