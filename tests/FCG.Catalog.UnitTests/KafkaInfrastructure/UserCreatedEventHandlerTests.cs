using System.Text.Json;
using FCG.Catalog.Application.UseCases.Libraries.Register;
using FCG.Catalog.Infrastructure.Kafka.Consumers.Handlers;
using FCG.Catalog.Infrastructure.Kafka.Messages;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace FCG.Catalog.UnitTests.KafkaInfrastructure
{
    public class UserCreatedEventHandlerTests
    {
        private readonly Mock<IServiceScopeFactory> _serviceScopeFactoryMock;
        private readonly Mock<IServiceScope> _serviceScopeMock;
        private readonly Mock<IServiceProvider> _serviceProviderMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ILogger<UserCreatedEventHandler>> _loggerMock;
        private readonly UserCreatedEventHandler _handler;

        public UserCreatedEventHandlerTests()
        {
            _serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
            _serviceScopeMock = new Mock<IServiceScope>();
            _serviceProviderMock = new Mock<IServiceProvider>();
            _mediatorMock = new Mock<IMediator>();
            _loggerMock = new Mock<ILogger<UserCreatedEventHandler>>();

            _serviceScopeMock.Setup(x => x.ServiceProvider).Returns(_serviceProviderMock.Object);
            _serviceScopeFactoryMock.Setup(x => x.CreateScope()).Returns(_serviceScopeMock.Object);
            _serviceProviderMock.Setup(x => x.GetService(typeof(IMediator))).Returns(_mediatorMock.Object);

            _handler = new UserCreatedEventHandler(_serviceScopeFactoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void Topic_ShouldReturnCorrectTopicName()
        {
            // Assert
            _handler.Topic.Should().Be("user-created");
        }

        [Fact]
        public async Task HandleAsync_ShouldCreateEmptyLibrary_WhenUserCreated()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var message = new UserCreatedMessage
            {
                UserId = userId,
                Name = "John Doe",
                Email = "john@example.com",
                CorrelationId = Guid.NewGuid(),
                OccurredAt = DateTime.UtcNow
            };

            var messageJson = JsonSerializer.Serialize(message, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            CreateEmptyLibraryCommand? capturedCommand = null;

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<CreateEmptyLibraryCommand>(), It.IsAny<CancellationToken>()))
                .Callback<CreateEmptyLibraryCommand, CancellationToken>((cmd, _) =>
                {
                    capturedCommand = cmd;
                })
                .Returns(Task.FromResult(Unit.Value));

            // Act
            await _handler.HandleAsync(messageJson, CancellationToken.None);

            // Assert
            capturedCommand.Should().NotBeNull();
            capturedCommand!.UserId.Should().Be(userId);
        }


        [Fact]
        public async Task HandleAsync_ShouldCallMediator_WhenMessageIsValid()
        {
            // Arrange
            var message = new UserCreatedMessage
            {
                UserId = Guid.NewGuid(),
                Name = "Jane Smith",
                Email = "jane@example.com",
                CorrelationId = Guid.NewGuid(),
                OccurredAt = DateTime.UtcNow
            };

            var messageJson = JsonSerializer.Serialize(message, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<CreateEmptyLibraryCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(Unit.Value));

            // Act
            await _handler.HandleAsync(messageJson, CancellationToken.None);

            // Assert
            _mediatorMock.Verify(
                x => x.Send(It.IsAny<CreateEmptyLibraryCommand>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task HandleAsync_ShouldThrowException_WhenMessageIsInvalid()
        {
            // Arrange
            var invalidJson = "{ invalid json }";

            // Act
            Func<Task> act = async () => await _handler.HandleAsync(invalidJson, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<JsonException>();
        }

        [Fact]
        public async Task HandleAsync_ShouldThrowException_WhenMessageIsNull()
        {
            // Arrange
            var nullJson = "null";

            // Act
            Func<Task> act = async () => await _handler.HandleAsync(nullJson, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Mensagem invalida ou nula.");
        }

        [Fact]
        public async Task HandleAsync_ShouldRespectCancellationToken_WhenCancelled()
        {
            // Arrange
            var message = new UserCreatedMessage
            {
                UserId = Guid.NewGuid(),
                Name = "Test User",
                Email = "test@example.com",
                CorrelationId = Guid.NewGuid(),
                OccurredAt = DateTime.UtcNow
            };

            var messageJson = JsonSerializer.Serialize(message, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var cts = new CancellationTokenSource();
            cts.Cancel();

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<CreateEmptyLibraryCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new OperationCanceledException());

            // Act
            Func<Task> act = async () => await _handler.HandleAsync(messageJson, cts.Token);

            // Assert
            await act.Should().ThrowAsync<OperationCanceledException>();
        }

        [Fact]
        public async Task HandleAsync_ShouldCreateScope_WhenProcessingMessage()
        {
            // Arrange
            var message = new UserCreatedMessage
            {
                UserId = Guid.NewGuid(),
                Name = "Bob Johnson",
                Email = "bob@example.com",
                CorrelationId = Guid.NewGuid(),
                OccurredAt = DateTime.UtcNow
            };

            var messageJson = JsonSerializer.Serialize(message, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<CreateEmptyLibraryCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(Unit.Value));

            // Act
            await _handler.HandleAsync(messageJson, CancellationToken.None);

            // Assert
            _serviceScopeFactoryMock.Verify(x => x.CreateScope(), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_ShouldMapUserIdCorrectly_WhenCreatingCommand()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var message = new UserCreatedMessage
            {
                UserId = userId,
                Name = "Alice Williams",
                Email = "alice@example.com",
                CorrelationId = Guid.NewGuid(),
                OccurredAt = DateTime.UtcNow
            };

            var messageJson = JsonSerializer.Serialize(message, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            CreateEmptyLibraryCommand? capturedCommand = null;
            _mediatorMock
                .Setup(x => x.Send(It.IsAny<CreateEmptyLibraryCommand>(), It.IsAny<CancellationToken>()))
                .Callback<IRequest, CancellationToken>((cmd, _) => capturedCommand = cmd as CreateEmptyLibraryCommand)
                .Returns(Task.FromResult(Unit.Value));

            // Act
            await _handler.HandleAsync(messageJson, CancellationToken.None);

            // Assert
            capturedCommand.Should().NotBeNull();
            capturedCommand!.UserId.Should().Be(userId);
        }

        [Fact]
        public async Task HandleAsync_ShouldPropagateException_WhenMediatorFails()
        {
            // Arrange
            var message = new UserCreatedMessage
            {
                UserId = Guid.NewGuid(),
                Name = "Charlie Brown",
                Email = "charlie@example.com",
                CorrelationId = Guid.NewGuid(),
                OccurredAt = DateTime.UtcNow
            };

            var messageJson = JsonSerializer.Serialize(message, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var expectedException = new InvalidOperationException("Mediator failed");
            _mediatorMock
                .Setup(x => x.Send(It.IsAny<CreateEmptyLibraryCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(expectedException);

            // Act
            Func<Task> act = async () => await _handler.HandleAsync(messageJson, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Mediator failed");
        }

        [Fact]
        public async Task HandleAsync_ShouldDeserializeMessageCaseInsensitively_WhenPropertyNamesVary()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var messageJson = $@"{{
                ""userid"": ""{userId}"",
                ""NAME"": ""Test User"",
                ""EmAiL"": ""test@example.com"",
                ""correlationid"": ""{Guid.NewGuid()}"",
                ""occurredat"": ""{DateTime.UtcNow:O}""
            }}";

            CreateEmptyLibraryCommand? capturedCommand = null;
            _mediatorMock
                .Setup(x => x.Send(It.IsAny<CreateEmptyLibraryCommand>(), It.IsAny<CancellationToken>()))
                .Callback<IRequest, CancellationToken>((cmd, _) => capturedCommand = cmd as CreateEmptyLibraryCommand)
                .Returns(Task.FromResult(Unit.Value));

            // Act
            await _handler.HandleAsync(messageJson, CancellationToken.None);

            // Assert
            capturedCommand.Should().NotBeNull();
            capturedCommand!.UserId.Should().Be(userId);
        }

        [Theory]
        [InlineData("user1@test.com", "User One")]
        [InlineData("user2@test.com", "User Two")]
        [InlineData("user3@test.com", "User Three")]
        public async Task HandleAsync_ShouldProcessDifferentUsers_WhenCalled(string email, string name)
        {
            // Arrange
            var message = new UserCreatedMessage
            {
                UserId = Guid.NewGuid(),
                Name = name,
                Email = email,
                CorrelationId = Guid.NewGuid(),
                OccurredAt = DateTime.UtcNow
            };

            var messageJson = JsonSerializer.Serialize(message, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<CreateEmptyLibraryCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(Unit.Value));

            // Act
            await _handler.HandleAsync(messageJson, CancellationToken.None);

            // Assert
            _mediatorMock.Verify(
                x => x.Send(It.IsAny<CreateEmptyLibraryCommand>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}