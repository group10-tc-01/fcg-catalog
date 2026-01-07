using FCG.Catalog.Application.UseCases.Games.Delete;
using FCG.Catalog.CommomTestUtilities.Builders;
using FCG.Catalog.CommomTestUtilities.Builders.Games;
using FCG.Catalog.CommomTestUtilities.Builders.Games.Repositories;
using FCG.Catalog.Domain.Abstractions;
using FCG.Catalog.Domain.Exception;
using FCG.Catalog.Domain.Repositories.Game;
using FCG.Catalog.Messages;
using FluentAssertions;
using MediatR;
using Moq;

namespace FCG.Catalog.UnitTests.Application.UseCases.Games
{
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
        public async Task Handle_ShouldDeleteGame_WhenGameExists()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var game = _gameBuilder.Build();
            var input = new DeleteGameInput(gameId);

            ReadOnlyGameRepositoryBuilder.SetupGetByIdAsync(gameId, game);
            ReadOnlyGameRepositoryBuilder.SetupDelete(game);
            UnitOfWorkBuilder.SetupCommitTransactionAsync();
            UnitOfWorkBuilder.SetupSaveChangesAsync();

            var useCase = new DeleteGameUseCase(
                ReadOnlyGameRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build()
            );

            // Act
            var result = await useCase.Handle(input, CancellationToken.None);

            // Assert
            result.Should().Be(Unit.Value);
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
        public async Task Handle_ShouldCallGetByIdAsync_WithCorrectId()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var game = _gameBuilder.Build();
            var input = new DeleteGameInput(gameId);

            ReadOnlyGameRepositoryBuilder.SetupGetByIdAsync(gameId, game);
            ReadOnlyGameRepositoryBuilder.SetupDelete(game);
            UnitOfWorkBuilder.SetupCommitTransactionAsync();
            UnitOfWorkBuilder.SetupSaveChangesAsync();

            var useCase = new DeleteGameUseCase(
                ReadOnlyGameRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build()
            );

            // Act
            await useCase.Handle(input, CancellationToken.None);

            // Assert
            ReadOnlyGameRepositoryBuilder.VerifyGetByIdAsyncWasCalled(gameId, Times.Once());
        }

        [Fact]
        public async Task Handle_ShouldCommitTransaction_AfterDelete()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var game = _gameBuilder.Build();
            var input = new DeleteGameInput(gameId);

            ReadOnlyGameRepositoryBuilder.SetupGetByIdAsync(gameId, game);
            ReadOnlyGameRepositoryBuilder.SetupDelete(game);
            UnitOfWorkBuilder.SetupCommitTransactionAsync();
            UnitOfWorkBuilder.SetupSaveChangesAsync();

            var useCase = new DeleteGameUseCase(
                ReadOnlyGameRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build()
            );

            // Act
            await useCase.Handle(input, CancellationToken.None);

            // Assert
            UnitOfWorkBuilder.VerifyCommitTransactionAsyncWasCalled(Times.Once());
        }

        [Fact]
        public async Task Handle_ShouldSaveChanges_AfterCommit()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var game = _gameBuilder.Build();
            var input = new DeleteGameInput(gameId);

            ReadOnlyGameRepositoryBuilder.SetupGetByIdAsync(gameId, game);
            ReadOnlyGameRepositoryBuilder.SetupDelete(game);
            UnitOfWorkBuilder.SetupCommitTransactionAsync();
            UnitOfWorkBuilder.SetupSaveChangesAsync();

            var useCase = new DeleteGameUseCase(
                ReadOnlyGameRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build()
            );

            // Act
            await useCase.Handle(input, CancellationToken.None);

            // Assert
            UnitOfWorkBuilder.VerifySaveChangesAsyncWasCalled(Times.Once());
        }

        [Fact]
        public async Task Handle_ShouldNotCommitOrSave_WhenGameNotFound()
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
            await act.Should().ThrowAsync<NotFoundException>();
            UnitOfWorkBuilder.VerifyCommitTransactionAsyncWasNotCalled();
            UnitOfWorkBuilder.VerifySaveChangesAsyncWasNotCalled();
        }

        [Fact]
        public async Task Handle_ShouldRespectCancellationToken()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var game = _gameBuilder.Build();
            var input = new DeleteGameInput(gameId);
            var cancellationToken = new CancellationToken();

            ReadOnlyGameRepositoryBuilder.SetupGetByIdAsync(gameId, game);
            ReadOnlyGameRepositoryBuilder.SetupDelete(game);
            UnitOfWorkBuilder.SetupCommitTransactionAsync();
            UnitOfWorkBuilder.SetupSaveChangesAsync();

            var useCase = new DeleteGameUseCase(
                ReadOnlyGameRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build()
            );

            // Act
            var result = await useCase.Handle(input, cancellationToken);

            // Assert
            result.Should().Be(Unit.Value);
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

        [Fact]
        public async Task Handle_ShouldPropagateException_WhenCommitFails()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var game = _gameBuilder.Build();
            var input = new DeleteGameInput(gameId);
            var expectedException = new Exception("Commit failed");

            ReadOnlyGameRepositoryBuilder.SetupGetByIdAsync(gameId, game);
            ReadOnlyGameRepositoryBuilder.SetupDelete(game);
            UnitOfWorkBuilder.SetupCommitTransactionAsyncThrows(expectedException);

            var useCase = new DeleteGameUseCase(
                ReadOnlyGameRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build()
            );

            // Act
            Func<Task> act = async () => await useCase.Handle(input, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Commit failed");
        }

        [Fact]
        public async Task Handle_ShouldPropagateException_WhenSaveFails()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var game = _gameBuilder.Build();
            var input = new DeleteGameInput(gameId);
            var expectedException = new Exception("Save failed");

            ReadOnlyGameRepositoryBuilder.SetupGetByIdAsync(gameId, game);
            ReadOnlyGameRepositoryBuilder.SetupDelete(game);
            UnitOfWorkBuilder.SetupCommitTransactionAsync();
            UnitOfWorkBuilder.SetupSaveChangesAsyncThrows(expectedException);

            var useCase = new DeleteGameUseCase(
                ReadOnlyGameRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build()
            );

            // Act
            Func<Task> act = async () => await useCase.Handle(input, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Save failed");
        }

        [Fact]
        public async Task Handle_ShouldReturnAffectedRows_FromSaveChanges()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var game = _gameBuilder.Build();
            var input = new DeleteGameInput(gameId);

            ReadOnlyGameRepositoryBuilder.SetupGetByIdAsync(gameId, game);
            ReadOnlyGameRepositoryBuilder.SetupDelete(game);
            UnitOfWorkBuilder.SetupCommitTransactionAsync();
            UnitOfWorkBuilder.SetupSaveChangesAsync(affectedRows: 1);

            var useCase = new DeleteGameUseCase(
                ReadOnlyGameRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build()
            );

            // Act
            var result = await useCase.Handle(input, CancellationToken.None);

            // Assert
            result.Should().Be(Unit.Value);
            UnitOfWorkBuilder.VerifySaveChangesAsyncWasCalled(Times.Once());
        }

        [Fact]
        public async Task Handle_ShouldDeleteDifferentGames_WithDifferentIds()
        {
            // Arrange
            var gameId1 = Guid.NewGuid();
            var gameId2 = Guid.NewGuid();
            var game1 = _gameBuilder.BuildWithName("Game 1");
            var game2 = _gameBuilder.BuildWithName("Game 2");

            var input1 = new DeleteGameInput(gameId1);
            var input2 = new DeleteGameInput(gameId2);

            ReadOnlyGameRepositoryBuilder.SetupGetByIdAsync(gameId1, game1);
            ReadOnlyGameRepositoryBuilder.SetupGetByIdAsync(gameId2, game2);
            ReadOnlyGameRepositoryBuilder.SetupDelete(game1);
            ReadOnlyGameRepositoryBuilder.SetupDelete(game2);
            UnitOfWorkBuilder.SetupCommitTransactionAsync();
            UnitOfWorkBuilder.SetupSaveChangesAsync();

            var useCase = new DeleteGameUseCase(
                ReadOnlyGameRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build()
            );

            // Act
            var result1 = await useCase.Handle(input1, CancellationToken.None);
            var result2 = await useCase.Handle(input2, CancellationToken.None);

            // Assert
            result1.Should().Be(Unit.Value);
            result2.Should().Be(Unit.Value);
            ReadOnlyGameRepositoryBuilder.VerifyGetByIdAsyncWasCalled(gameId1, Times.Once());
            ReadOnlyGameRepositoryBuilder.VerifyGetByIdAsyncWasCalled(gameId2, Times.Once());
        }
    }
}