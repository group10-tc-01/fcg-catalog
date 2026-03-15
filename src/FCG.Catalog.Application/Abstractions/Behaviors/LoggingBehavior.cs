using FCG.Catalog.Application.Abstractions.Messaging;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace FCG.Catalog.Application.Abstractions.Behaviors
{
    [ExcludeFromCodeCoverage]
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : ICommand<TResponse>
    {
        private readonly ILogger<TRequest> _logger;

        public LoggingBehavior(ILogger<TRequest> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var name = request.GetType().Name;

            try
            {
                _logger.LogInformation("Executing command {Command}", name);

                var result = await next(cancellationToken);

                _logger.LogInformation("Command {Command} processed successfully", name);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Command {Command} processing failed", name);

                throw;
            }
        }
    }
}
