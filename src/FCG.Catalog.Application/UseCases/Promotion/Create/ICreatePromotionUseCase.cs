using MediatR;

namespace FCG.Catalog.Application.UseCases.Promotion.Create
{
    public interface ICreatePromotionUseCase : IRequestHandler<CreatePromotionRequest, CreatePromotionResponse> { }

}
