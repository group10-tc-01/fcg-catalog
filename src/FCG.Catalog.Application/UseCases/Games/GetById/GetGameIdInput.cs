using MediatR;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
