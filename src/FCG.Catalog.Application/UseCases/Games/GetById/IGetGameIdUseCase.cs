using MediatR;

namespace FCG.Catalog.Application.UseCases.Games.GetById
{
    public interface IGetGameIdUseCase : IRequestHandler<GetGameIdInput, GetGameIdOutput>
    {
    }
}
