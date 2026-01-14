using FCG.Catalog.Domain.Models;
using MediatR;

namespace FCG.Catalog.Application.UseCases.Games.Get
{
    public interface IGetAllGamesUseCase : IRequestHandler<GetGameInput, PagedListResponse<GetGameOutput>>
    {
    }
}
