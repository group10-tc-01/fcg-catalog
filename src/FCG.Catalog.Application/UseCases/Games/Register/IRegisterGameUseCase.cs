using FCG.Catalog.Domain.Repositories.Game;
using MediatR;

namespace FCG.Catalog.Application.UseCases.Games.Register
{
    public interface IRegisterGameUseCase : IRequestHandler<RegisterGameInput, RegisterGameOutput>
    {
    }

}
