using System.Threading;
using System.Threading.Tasks;

namespace FCG.Catalog.Application.Abstractions.Messaging
{
    public interface IMessageProducer
    {
        Task ProduceAsync<T>(string topic, T message, CancellationToken cancellationToken = default) where T : class;
    }
}