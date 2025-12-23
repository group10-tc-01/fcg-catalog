using MediatR;

namespace FCG.Catalog.Application.UseCases.Libraries.Get
{
    public record GetLibraryByUserIdQuery(string UserId) : IRequest<GetLibraryByUserIdResponse>
    {
    }
}
