using MediatR;

namespace FCG.Catalog.Application.UseCases.Promotion.Delete
{
    public interface IDeletePromotionUseCase : IRequestHandler<DeletePromotionInput, DeletePromotionOutput> { }
}
