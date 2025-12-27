using FCG.Catalog.Application.UseCases.Libraries.Register;
using FCG.Catalog.Infrastructure.Kafka.Messages;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FCG.Catalog.Infrastructure.Kafka.Consumers.Handlers
{
    public class UserCreatedEventHandler : KafkaConsumerBase<UserCreatedMessage, CreateEmptyLibraryCommand>
    {
        public UserCreatedEventHandler(IMediator mediator, ILogger<UserCreatedEventHandler> logger) : base(mediator, logger)
        {
        }

        public override string Topic => "user-created";

        protected override CreateEmptyLibraryCommand MapToCommand(UserCreatedMessage message)
        {
            return new CreateEmptyLibraryCommand()
            {
                UserId = message.UserId
            };
        }
    }
}
