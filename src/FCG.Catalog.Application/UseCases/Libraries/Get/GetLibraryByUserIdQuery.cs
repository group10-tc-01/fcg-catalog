using MediatR;

namespace FCG.Catalog.Application.UseCases.Libraries.Get
{
    public record GetLibraryByUserIdQuery(Guid UserId) : IRequest<GetLibraryByUserIdResponse>;
}
