using MediatR;

namespace FCG.Catalog.Application.Abstractions.Messaging
{
    [ExcludeFromCodeCoverage]

    public interface ICommand : IRequest { }

    public interface ICommand<TResponse> : IRequest<TResponse> { }
}