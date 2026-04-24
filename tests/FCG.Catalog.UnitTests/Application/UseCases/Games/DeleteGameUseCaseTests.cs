using FCG.Catalog.Application.UseCases.Games.Delete;
using FCG.Catalog.CommomTestUtilities.Builders;
using FCG.Catalog.CommomTestUtilities.Builders.Games;
using FCG.Catalog.CommomTestUtilities.Builders.Games.Repositories;
using FCG.Catalog.CommomTestUtilities.Builders.Promotions.Repositories;
using FCG.Catalog.Domain.Abstractions;
using FCG.Catalog.Domain.Catalog.Entities.Promotions;
using FCG.Catalog.Domain.Exception;
using FCG.Catalog.Domain.Repositories.Game;
using FCG.Catalog.Domain.Repositories.Promotion;
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
                ReadOnlyPromotionRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build(),
                Mock.Of<IGameSearchRepository>()
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
            
            var mockPromoRepo = new Mock<IReadOnlyPromotionRepository>();
            mockPromoRepo.Setup(r => r.GetByGameIdAsync(gameId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Promotion>())
                .Callback(() => callOrder.Add("GetByPromotion"));

            var mockUow = new Mock<IUnitOfWork>();
            mockUow.Setup(u => u.CommitTransactionAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Callback(() => callOrder.Add("Commit"));

            mockUow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1)
                .Callback(() => callOrder.Add("Save"));

            var useCase = new DeleteGameUseCase(
                mockRepo.Object, 
                mockPromoRepo.Object, 
                mockUow.Object,
                Mock.Of<IGameSearchRepository>());
            // Act
            await useCase.Handle(input, CancellationToken.None);

            // Assert
            callOrder.Should().Equal("GetById", "GetByPromotion", "Delete", "Commit", "Save");        }

        [Fact]
        public async Task Handle_ShouldDeleteGameFromSearchIndexAfterSaving_WhenGameIsDeleted()
        {
            // Arrange
            var game = _gameBuilder.Build();
            var input = new DeleteGameInput(game.Id);

            var gameRepositoryMock = new Mock<IReadOnlyGameRepository>();
            gameRepositoryMock
                .Setup(x => x.GetByIdAsync(game.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(game);
            gameRepositoryMock
                .Setup(x => x.Delete(game, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var promotionRepositoryMock = new Mock<IReadOnlyPromotionRepository>();
            promotionRepositoryMock
                .Setup(x => x.GetByGameIdAsync(game.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Promotion>());

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock
                .Setup(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var searchRepositoryMock = new Mock<IGameSearchRepository>();

            var useCase = new DeleteGameUseCase(
                gameRepositoryMock.Object,
                promotionRepositoryMock.Object,
                unitOfWorkMock.Object,
                searchRepositoryMock.Object);

            // Act
            await useCase.Handle(input, CancellationToken.None);

            // Assert
            searchRepositoryMock.Verify(
                x => x.DeleteAsync(game.Id, It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
