using FCG.Catalog.Application.UseCases.Games.Delete;
using FCG.Catalog.CommomTestUtilities.Builders;
using FCG.Catalog.CommomTestUtilities.Builders.Games;
using FCG.Catalog.CommomTestUtilities.Builders.Games.Repositories;
using FCG.Catalog.Domain.Abstractions;
using FCG.Catalog.Domain.Exception;
using FCG.Catalog.Domain.Repositories.Game;
using FCG.Catalog.Messages;
using FluentAssertions;
using Moq;

namespace FCG.Catalog.UnitTests.Application.UseCases.Games
{
    [Collection("Sequential")]
    public class DeleteGameUseCaseTests
    {
        private readonly GameBuilder _gameBuilder;

        public DeleteGameUseCaseTests()
        {
            _gameBuilder = new GameBuilder();
            ReadOnlyGameRepositoryBuilder.Reset();
            UnitOfWorkBuilder.Reset();
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenGameDoesNotExist()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var input = new DeleteGameInput(gameId);

            ReadOnlyGameRepositoryBuilder.SetupGetByIdAsync(gameId, null);

            var useCase = new DeleteGameUseCase(
                ReadOnlyGameRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build()
            );

            // Act
            Func<Task> act = async () => await useCase.Handle(input, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage(ResourceMessages.GameNotFound);
        }

        [Fact]
        public async Task Handle_ShouldExecuteInCorrectOrder()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var game = _gameBuilder.Build();
            var input = new DeleteGameInput(gameId);
            var callOrder = new List<string>();

            var mockRepo = new Mock<IReadOnlyGameRepository>();
            mockRepo.Setup(r => r.GetByIdAsync(gameId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(game)
                .Callback(() => callOrder.Add("GetById"));

            mockRepo.Setup(r => r.Delete(game, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Callback(() => callOrder.Add("Delete"));

            var mockUow = new Mock<IUnitOfWork>();
            mockUow.Setup(u => u.CommitTransactionAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Callback(() => callOrder.Add("Commit"));

            mockUow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1)
                .Callback(() => callOrder.Add("Save"));

            var useCase = new DeleteGameUseCase(mockRepo.Object, mockUow.Object);

            // Act
            await useCase.Handle(input, CancellationToken.None);

            // Assert
            callOrder.Should().Equal("GetById", "Delete", "Commit", "Save");
        }
    }
}