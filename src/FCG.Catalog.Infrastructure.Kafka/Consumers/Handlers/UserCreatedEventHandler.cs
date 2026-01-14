using FCG.Catalog.Application.UseCases.Libraries.Register;
using FCG.Catalog.Infrastructure.Kafka.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FCG.Catalog.Infrastructure.Kafka.Consumers.Handlers
{
    public class UserCreatedEventHandler : KafkaConsumerBase<UserCreatedMessage, CreateEmptyLibraryCommand>
    {
        public UserCreatedEventHandler(
            IServiceScopeFactory serviceScopeFactory,
            ILogger<UserCreatedEventHandler> logger)
            : base(serviceScopeFactory, logger)
        {
        }
        public override string Topic => "user-created";

        protected override CreateEmptyLibraryCommand MapToCommand(UserCreatedMessage message)
        {
            return new CreateEmptyLibraryCommand(message.UserId);
        }
    }
}
