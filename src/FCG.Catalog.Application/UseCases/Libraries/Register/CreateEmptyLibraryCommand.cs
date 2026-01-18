using MediatR;

namespace FCG.Catalog.Application.UseCases.Libraries.Register
{
    public record CreateEmptyLibraryCommand(Guid UserId) : IRequest;
}
