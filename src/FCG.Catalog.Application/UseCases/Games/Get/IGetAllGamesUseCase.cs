using FCG.Catalog.Domain.Models;
using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace FCG.Catalog.Application.UseCases.Games.Get
{
    public interface IGetAllGamesUseCase  : IRequestHandler<GetGameInput, PagedListResponse<GetGameOutput>>
    {
    }
}
