using System.Text.Json;
using FCG.Catalog.Application.UseCases.Games.ProcessPayment;
using FCG.Catalog.Infrastructure.Kafka.Consumers.Handlers;
using FCG.Catalog.Infrastructure.Kafka.Messages;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace FCG.Catalog.UnitTests.KafkaInfrastructure
{
    public class PaymentProcessedEventHandlerTests
    {
        private readonly Mock<IServiceScopeFactory> _serviceScopeFactoryMock;
        private readonly Mock<IServiceScope> _serviceScopeMock;
        private readonly Mock<IServiceProvider> _serviceProviderMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ILogger<PaymentProcessedEventHandler>> _loggerMock;
        private readonly PaymentProcessedEventHandler _handler;

        public PaymentProcessedEventHandlerTests()
        {
            _serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
            _serviceScopeMock = new Mock<IServiceScope>();
            _serviceProviderMock = new Mock<IServiceProvider>();
            _mediatorMock = new Mock<IMediator>();
            _loggerMock = new Mock<ILogger<PaymentProcessedEventHandler>>();

            _serviceScopeMock.Setup(x => x.ServiceProvider).Returns(_serviceProviderMock.Object);
            _serviceScopeFactoryMock.Setup(x => x.CreateScope()).Returns(_serviceScopeMock.Object);
            _serviceProviderMock.Setup(x => x.GetService(typeof(IMediator))).Returns(_mediatorMock.Object);

            _handler = new PaymentProcessedEventHandler(_serviceScopeFactoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void Topic_ShouldReturnCorrectTopicName()
        {
            // Assert
            _handler.Topic.Should().Be("payment-processed");
        }

        [Fact]
        public async Task HandleAsync_WithApprovedStatus_ShouldSendCommandWithIsApprovedTrue()
        {
            // Arrange
            var correlationId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var gameId = Guid.NewGuid();
            var amount = 59.99m;

            var message = new PaymentProcessedMessage
            {
                CorrelationId = correlationId,
                PaymentId = Guid.NewGuid(),
                UserId = userId,
                GameId = gameId,
                Amount = amount,
                Status = "Approved",
                ProcessedAt = DateTime.UtcNow
            };

            var messageJson = JsonSerializer.Serialize(message, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<ProcessPaymentResultInput>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(Unit.Value));

            // Act
            await _handler.HandleAsync(messageJson, CancellationToken.None);

            // Assert
            _mediatorMock
                .Invocations
                .Where(inv => inv.Method.Name == nameof(IMediator.Send))
                .Should().ContainSingle()
                .Which.Arguments[0].Should().BeOfType<ProcessPaymentResultInput>()
                .Which.Should().Match<ProcessPaymentResultInput>(cmd =>
                    cmd.CorrelationId == correlationId &&
                    cmd.UserId == userId &&
                    cmd.GameId == gameId &&
                    cmd.Amount == amount &&
                    cmd.IsApproved == true);
        }

        [Fact]
        public async Task HandleAsync_WithRejectedStatus_ShouldSendCommandWithIsApprovedFalse()
        {
            // Arrange
            var message = new PaymentProcessedMessage
            {
                CorrelationId = Guid.NewGuid(),
                PaymentId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                GameId = Guid.NewGuid(),
                Amount = 99.99m,
                Status = "Rejected",
                ProcessedAt = DateTime.UtcNow
            };

            var messageJson = JsonSerializer.Serialize(message, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<ProcessPaymentResultInput>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(Unit.Value));

            // Act
            await _handler.HandleAsync(messageJson, CancellationToken.None);

            // Assert
            _mediatorMock
                .Invocations
                .Where(inv => inv.Method.Name == nameof(IMediator.Send))
                .Should().ContainSingle()
                .Which.Arguments[0].Should().BeOfType<ProcessPaymentResultInput>()
                .Which.IsApproved.Should().BeFalse();
        }

        [Theory]
        [InlineData("approved")]
        [InlineData("APPROVED")]
        [InlineData("Approved")]
        [InlineData("ApProVeD")]
        public async Task HandleAsync_ShouldTreatStatusAsCaseInsensitive_WhenCheckingApproval(string status)
        {
            // Arrange
            var message = new PaymentProcessedMessage
            {
                CorrelationId = Guid.NewGuid(),
                PaymentId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                GameId = Guid.NewGuid(),
                Amount = 75m,
                Status = status,
                ProcessedAt = DateTime.UtcNow
            };

            var messageJson = JsonSerializer.Serialize(message, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<ProcessPaymentResultInput>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(Unit.Value));

            // Act
            await _handler.HandleAsync(messageJson, CancellationToken.None);

            // Assert
            _mediatorMock
                .Invocations
                .Where(inv => inv.Method.Name == nameof(IMediator.Send))
                .Should().ContainSingle()
                .Which.Arguments[0].Should().BeOfType<ProcessPaymentResultInput>()
                .Which.IsApproved.Should().BeTrue();
        }

        [Fact]
        public async Task HandleAsync_WithValidMessage_ShouldLogInformation()
        {
            // Arrange
            var message = new PaymentProcessedMessage
            {
                CorrelationId = Guid.NewGuid(),
                PaymentId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                GameId = Guid.NewGuid(),
                Amount = 120m,
                Status = "Approved",
                ProcessedAt = DateTime.UtcNow
            };

            var messageJson = JsonSerializer.Serialize(message, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<ProcessPaymentResultInput>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(Unit.Value));

            // Act
            await _handler.HandleAsync(messageJson, CancellationToken.None);

            // Assert
            _loggerMock
                .Invocations
                .Should().Contain(inv =>
                    inv.Method.Name == "Log" &&
                    inv.Arguments[0].Equals(LogLevel.Information) &&
                    inv.Arguments[2].ToString()!.Contains("Processando mensagem do tópico payment-processed"));
        }

        [Fact]
        public async Task HandleAsync_WhenProcessingSucceeds_ShouldLogSuccess()
        {
            // Arrange
            var message = new PaymentProcessedMessage
            {
                CorrelationId = Guid.NewGuid(),
                PaymentId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                GameId = Guid.NewGuid(),
                Amount = 89.99m,
                Status = "Approved",
                ProcessedAt = DateTime.UtcNow
            };

            var messageJson = JsonSerializer.Serialize(message, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<ProcessPaymentResultInput>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(Unit.Value));

            // Act
            await _handler.HandleAsync(messageJson, CancellationToken.None);

            // Assert
            _loggerMock
                .Invocations
                .Should().Contain(inv =>
                    inv.Method.Name == "Log" &&
                    inv.Arguments[0].Equals(LogLevel.Information) &&
                    inv.Arguments[2].ToString()!.Contains("Mensagem processada com sucesso no tópico payment-processed"));
        }

        [Fact]
        public async Task HandleAsync_WithInvalidJson_ShouldLogErrorAndThrowException()
        {
            // Arrange
            var invalidJson = "{ invalid json }";

            // Act
            var act = async () => await _handler.HandleAsync(invalidJson, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<JsonException>();

            _loggerMock
                .Invocations
                .Should().Contain(inv =>
                    inv.Method.Name == "Log" &&
                    inv.Arguments[0].Equals(LogLevel.Error) &&
                    inv.Arguments[2].ToString()!.Contains("Erro ao processar mensagem do tópico payment-processed"));
        }

        [Fact]
        public async Task HandleAsync_WithNullMessage_ShouldLogErrorAndThrowException()
        {
            // Arrange
            var nullJson = "null";

            // Act
            var act = async () => await _handler.HandleAsync(nullJson, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Mensagem invalida ou nula.");

            _loggerMock
                .Invocations
                .Should().Contain(inv =>
                    inv.Method.Name == "Log" &&
                    inv.Arguments[0].Equals(LogLevel.Error) &&
                    inv.Arguments[2].ToString()!.Contains("Erro ao processar mensagem do tópico payment-processed"));
        }

        [Fact]
        public async Task HandleAsync_WhenMediatorThrowsException_ShouldLogErrorAndRethrow()
        {
            // Arrange
            var message = new PaymentProcessedMessage
            {
                CorrelationId = Guid.NewGuid(),
                PaymentId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                GameId = Guid.NewGuid(),
                Amount = 200m,
                Status = "Approved",
                ProcessedAt = DateTime.UtcNow
            };

            var messageJson = JsonSerializer.Serialize(message, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var expectedException = new InvalidOperationException("Mediator failed");
            _mediatorMock
                .Setup(x => x.Send(It.IsAny<ProcessPaymentResultInput>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(expectedException);

            // Act
            var act = async () => await _handler.HandleAsync(messageJson, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Mediator failed");

            _loggerMock
                .Invocations
                .Should().Contain(inv =>
                    inv.Method.Name == "Log" &&
                    inv.Arguments[0].Equals(LogLevel.Error) &&
                    inv.Arguments[2].ToString()!.Contains("Erro ao processar mensagem do tópico payment-processed") &&
                    inv.Arguments[3] == expectedException);
        }

        [Fact]
        public async Task HandleAsync_ShouldCreateScope_WhenProcessingMessage()
        {
            // Arrange
            var message = new PaymentProcessedMessage
            {
                CorrelationId = Guid.NewGuid(),
                PaymentId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                GameId = Guid.NewGuid(),
                Amount = 150m,
                Status = "Approved",
                ProcessedAt = DateTime.UtcNow
            };

            var messageJson = JsonSerializer.Serialize(message, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<ProcessPaymentResultInput>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(Unit.Value));

            // Act
            await _handler.HandleAsync(messageJson, CancellationToken.None);

            // Assert
            _serviceScopeFactoryMock.Verify(x => x.CreateScope(), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_ShouldCompleteSuccessfully_WhenValidMessage()
        {
            // Arrange
            var message = new PaymentProcessedMessage
            {
                CorrelationId = Guid.NewGuid(),
                PaymentId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                GameId = Guid.NewGuid(),
                Amount = 99.99m,
                Status = "Approved",
                ProcessedAt = DateTime.UtcNow
            };

            var messageJson = JsonSerializer.Serialize(message, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<ProcessPaymentResultInput>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(Unit.Value));

            // Act
            var act = async () => await _handler.HandleAsync(messageJson, CancellationToken.None);

            // Assert
            await act.Should().NotThrowAsync();
        }
    }
}