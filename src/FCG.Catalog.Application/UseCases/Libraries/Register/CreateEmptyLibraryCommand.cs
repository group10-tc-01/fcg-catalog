using MediatR;

namespace FCG.Catalog.Application.UseCases.Libraries.Register
{
    public record CreateEmptyLibraryCommand : IRequest
    {
        public Guid UserId { get; set; }
    }
}
