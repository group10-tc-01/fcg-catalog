using MediatR;

namespace FCG.Catalog.Application.Abstractions.Messaging
{
    public interface IQuery<TResponse> : IRequest<TResponse> { }
}