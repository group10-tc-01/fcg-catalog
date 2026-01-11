using MediatR;

namespace FCG.Catalog.Application.UseCases.Promotion.Update
{
    public interface IUpdatePromotionUseCase : IRequestHandler<UpdatePromotionInput, UpdatePromotionOutput> { }
}
