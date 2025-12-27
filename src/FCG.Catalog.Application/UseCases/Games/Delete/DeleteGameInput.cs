using MediatR;

namespace FCG.Catalog.Application.UseCases.Games.Delete
{
    public class DeleteGameInput : IRequest<Unit>
    {
        public Guid Id { get; private set; }
        public DeleteGameInput(Guid id)
        {
            Id = id;
        }
    }
}