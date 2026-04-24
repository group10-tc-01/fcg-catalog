using FCG.Catalog.Application.UseCases.Games.Update;
using FCG.Catalog.CommomTestUtilities.Builders;
using FCG.Catalog.CommomTestUtilities.Builders.Games.Repositories;
using FCG.Catalog.Domain.Abstractions;
using FCG.Catalog.Domain.Catalog.Entities.Games;
using FCG.Catalog.Domain.Catalog.ValueObjects;
using FCG.Catalog.Domain.Enum;
using FCG.Catalog.Domain.Exception;
using FCG.Catalog.Domain.Models;
using FCG.Catalog.Domain.Repositories.Game;
using FCG.Catalog.Messages;
using FCG.Catalog.UnitTests.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace FCG.Catalog.UnitTests.Application.UseCases.Games
{
    public class UpdateGameUseCaseTests
    {
        [Fact]
        public async Task Handle_ShouldUpdateGame_WhenGameExistsAndDataIsValid()
        {
            // Arrange
            var existingGame = Game.Create("Old Title", "Old Description", Price.Create(20m), GameCategory.Action);
            var gameId = existingGame.Id;
            var request = new UpdateGameInput
            {
                Id = gameId,
                Title = "New Title",
                Description = "New Description",
                Price = 30m,
                Category = GameCategory.RPG
            };

            var readRepoMock = new Mock<IReadOnlyGameRepository>();
            readRepoMock.Setup(repo => repo.GetByIdAsync(gameId, It.IsAny<CancellationToken>())).ReturnsAsync(existingGame);

            var writeRepoMock = new Mock<IWriteOnlyGameRepository>();
            writeRepoMock.Setup(repo => repo.Update(existingGame));

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);

            var useCase = new UpdateGameUseCase(
                readRepoMock.Object,
                writeRepoMock.Object,
                unitOfWorkMock.Object,
                Mock.Of<IGameSearchRepository>());

            // Act
            var result = await useCase.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(gameId);
            result.Title.Should().Be("New Title");
            result.Description.Should().Be("New Description");
            result.Price.Should().Be(30m);
            result.Category.Should().Be(GameCategory.RPG.ToString());
            writeRepoMock.Verify(repo => repo.Update(existingGame), Times.Once);
            unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenGameDoesNotExist()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var request = new UpdateGameInput
            {
                Id = gameId,
                Title = "Title",
                Description = "Description",
                Price = 10m,
                Category = GameCategory.Action
            };

            var readRepoMock = new Mock<IReadOnlyGameRepository>();
            readRepoMock.Setup(repo => repo.GetByIdAsync(gameId, It.IsAny<CancellationToken>())).ReturnsAsync((Game?)null);

            var writeRepoMock = new Mock<IWriteOnlyGameRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var useCase = new UpdateGameUseCase(
                readRepoMock.Object,
                writeRepoMock.Object,
                unitOfWorkMock.Object,
                Mock.Of<IGameSearchRepository>());

            // Act
            var act = () => useCase.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>().WithMessage($"Game id '{gameId}' not found.");
        }

        [Fact]
        public async Task Handle_ShouldThrowDomainException_WhenCategoryIsInvalid()
        {
            // Arrange
            var existingGame = Game.Create("Title", "Description", Price.Create(10m), GameCategory.Action);
            var gameId = existingGame.Id;
            var invalidCategory = (GameCategory)999; // Invalid enum value
            var request = new UpdateGameInput
            {
                Id = gameId,
                Title = "Title",
                Description = "Description",
                Price = 10m,
                Category = invalidCategory
            };

            var readRepoMock = new Mock<IReadOnlyGameRepository>();
            readRepoMock.Setup(repo => repo.GetByIdAsync(gameId, It.IsAny<CancellationToken>())).ReturnsAsync(existingGame);

            var writeRepoMock = new Mock<IWriteOnlyGameRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var useCase = new UpdateGameUseCase(
                readRepoMock.Object,
                writeRepoMock.Object,
                unitOfWorkMock.Object,
                Mock.Of<IGameSearchRepository>());

            // Act
            var act = () => useCase.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DomainException>().WithMessage($"Invalid category: '{invalidCategory}'. Available categories are: Action, Adventure, RPG...");
        }

        [Fact]
        public async Task Handle_ShouldThrowDomainException_WhenTitleIsInvalid()
        {
            // Arrange
            var existingGame = Game.Create("Valid Title", "Description", Price.Create(10m), GameCategory.Action);
            var gameId = existingGame.Id;
            var request = new UpdateGameInput
            {
                Id = gameId,
                Title = "", // Invalid title
                Description = "Description",
                Price = 10m,
                Category = GameCategory.Action
            };

            var readRepoMock = new Mock<IReadOnlyGameRepository>();
            readRepoMock.Setup(repo => repo.GetByIdAsync(gameId, It.IsAny<CancellationToken>())).ReturnsAsync(existingGame);

            var writeRepoMock = new Mock<IWriteOnlyGameRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var useCase = new UpdateGameUseCase(
                readRepoMock.Object,
                writeRepoMock.Object,
                unitOfWorkMock.Object,
                Mock.Of<IGameSearchRepository>());

            // Act
            var act = () => useCase.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DomainException>().WithMessage(ResourceMessages.GameNameIsRequired);
        }

        [Fact]
        public async Task Handle_ShouldThrowDomainException_WhenPriceIsNegative()
        {
            // Arrange
            var existingGame = Game.Create("Title", "Description", Price.Create(10m), GameCategory.Action);
            var gameId = existingGame.Id;
            var request = new UpdateGameInput
            {
                Id = gameId,
                Title = "Title",
                Description = "Description",
                Price = -5m, // Invalid price
                Category = GameCategory.Action
            };

            var readRepoMock = new Mock<IReadOnlyGameRepository>();
            readRepoMock.Setup(repo => repo.GetByIdAsync(gameId, It.IsAny<CancellationToken>())).ReturnsAsync(existingGame);

            var writeRepoMock = new Mock<IWriteOnlyGameRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var useCase = new UpdateGameUseCase(
                readRepoMock.Object,
                writeRepoMock.Object,
                unitOfWorkMock.Object,
                Mock.Of<IGameSearchRepository>());

            // Act
            var act = () => useCase.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DomainException>().WithMessage(ResourceMessages.GamePriceMustBeGreaterThanZero);
        }

        [Fact]
        public async Task Handle_ShouldThrowDomainException_WhenDescriptionIsInvalid()
        {
            // Arrange
            var existingGame = Game.Create("Title", "Description", Price.Create(10m), GameCategory.Action);
            var gameId = existingGame.Id;
            var request = new UpdateGameInput
            {
                Id = gameId,
                Title = "Title",
                Description = "", // Invalid description
                Price = 10m,
                Category = GameCategory.Action
            };

            var readRepoMock = new Mock<IReadOnlyGameRepository>();
            readRepoMock.Setup(repo => repo.GetByIdAsync(gameId, It.IsAny<CancellationToken>())).ReturnsAsync(existingGame);

            var writeRepoMock = new Mock<IWriteOnlyGameRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var useCase = new UpdateGameUseCase(
                readRepoMock.Object,
                writeRepoMock.Object,
                unitOfWorkMock.Object,
                Mock.Of<IGameSearchRepository>());

            // Act
            var act = () => useCase.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DomainException>().WithMessage(ResourceMessages.GameNameIsRequired); // Assuming description validation is similar
        }

        [Fact]
        public async Task Handle_ShouldUpdateSearchIndexAfterSaving_WhenGameIsUpdated()
        {
            // Arrange
            var existingGame = Game.Create("Old Title", "Old Description", Price.Create(20m), GameCategory.Action);
            var request = new UpdateGameInput
            {
                Id = existingGame.Id,
                Title = "Updated Indexed Title",
                Description = "Updated Indexed Description",
                Price = 45.5m,
                Category = GameCategory.Puzzle
            };

            var readRepoMock = new Mock<IReadOnlyGameRepository>();
            readRepoMock
                .Setup(repo => repo.GetByIdAsync(existingGame.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingGame);

            var writeRepoMock = new Mock<IWriteOnlyGameRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var searchRepositoryMock = new Mock<IGameSearchRepository>();

            var useCase = new UpdateGameUseCase(
                readRepoMock.Object,
                writeRepoMock.Object,
                unitOfWorkMock.Object,
                searchRepositoryMock.Object);

            // Act
            await useCase.Handle(request, CancellationToken.None);

            // Assert
            searchRepositoryMock.Verify(
                x => x.IndexAsync(
                    It.Is<GameSearch>(game =>
                        game.Id == existingGame.Id &&
                        game.Title == request.Title &&
                        game.Description == request.Description &&
                        game.Price == request.Price &&
                        game.DiscountedPrice == request.Price &&
                        game.Category == request.Category.ToString() &&
                        game.IsActive),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
