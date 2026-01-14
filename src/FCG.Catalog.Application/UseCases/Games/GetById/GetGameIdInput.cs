using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace FCG.Catalog.Application.UseCases.Games.GetById
{
    [ExcludeFromCodeCoverage]
    public class GetGameIdInput : IRequest<GetGameIdOutput>
    {
        public Guid Id { get; private set; }

        public GetGameIdInput(Guid id)
        {
            Id = id;
        }
    }

}
