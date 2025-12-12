using MediatR;
using System;

namespace FCG.Catalog.Application.UseCases.Games.Get
{
    public class GetGameInput : IRequest<GetGameOutput>
    {
        public Guid Id { get; init; }

        public GetGameInput(Guid id)
        {
            Id = id;
        }
    }
}
