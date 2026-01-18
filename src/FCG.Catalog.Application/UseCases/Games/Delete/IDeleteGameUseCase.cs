using MediatR;

namespace FCG.Catalog.Application.UseCases.Games.Delete
{
    public interface IDeleteGameUseCase : IRequestHandler<DeleteGameInput, Unit>
    {
    }
}