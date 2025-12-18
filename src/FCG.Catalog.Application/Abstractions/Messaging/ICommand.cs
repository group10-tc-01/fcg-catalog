using MediatR;

namespace FCG.Catalog.Application.Abstractions.Messaging
{
    public interface ICommand : IRequest { }

    public interface ICommand<TResponse> : IRequest<TResponse> { }
}