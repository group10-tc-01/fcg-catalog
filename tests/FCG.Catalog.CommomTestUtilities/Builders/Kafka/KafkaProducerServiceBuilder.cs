using FCG.Catalog.Infrastructure.Kafka.Services;
using FCG.Catalog.Infrastructure.Kafka.Services.Interfaces;
using Moq;

namespace FCG.Catalog.CommomTestUtilities.Builders.Kafka
{
    public static class KafkaProducerServiceBuilder
    {
        private static readonly Mock<IKafkaProducerService> _mock = new Mock<IKafkaProducerService>();

        public static IKafkaProducerService Build() => _mock.Object;

        public static void Reset() => _mock.Reset();

        public static void SetupPublishAsync<T>()
        {
            _mock.Setup(service => service.PublishAsync(
                    It.IsAny<string>(),
                    It.IsAny<T>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
        }

        public static void SetupPublishAsyncThrowsException<T>(Exception exception)
        {
            _mock.Setup(service => service.PublishAsync(
                    It.IsAny<string>(),
                    It.IsAny<T>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);
        }

        public static void VerifyPublishAsyncWasCalled<T>(string topicKey, Times times)
        {
            _mock.Verify(service => service.PublishAsync(
                topicKey,
                It.IsAny<T>(),
                It.IsAny<CancellationToken>()), times);
        }

        public static void VerifyPublishAsyncWasCalledWithMessage<T>(string topicKey, T message, Times times)
           {
               _mock.Verify(service => service.PublishAsync(
                   topicKey,
                   It.Is<T>(m => m != null && m.Equals(message)),
                   It.IsAny<CancellationToken>()), times);
           }
       }
   }
