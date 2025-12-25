using FCG.Catalog.Application.UseCases.Games.Register;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCG.Catalog.Application.UseCases.Games.GetById
{
    public interface IGetGameIdUseCase : IRequestHandler<GetGameIdInput, GetGameIdOutput>
    {
    }
}
