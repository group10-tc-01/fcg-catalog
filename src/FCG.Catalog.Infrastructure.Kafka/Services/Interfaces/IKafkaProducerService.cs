using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCG.Catalog.Infrastructure.Kafka.Services.Interfaces
{
    public interface IKafkaProducerService
    {
        Task PublishAsync<T>(string topicKey, T message, CancellationToken cancellationToken = default);
    }
}
