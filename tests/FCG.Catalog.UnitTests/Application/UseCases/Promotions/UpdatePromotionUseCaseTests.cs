using FCG.Catalog.Application.UseCases.Promotion.Update;
using FCG.Catalog.CommomTestUtilities.Builders.Games;
using FCG.Catalog.Domain.Abstractions;
using FCG.Catalog.Domain.Catalog.Entities.Promotions;
using FCG.Catalog.Domain.Catalog.ValueObjects;
using FCG.Catalog.Domain.Exception;
using FCG.Catalog.Domain.Repositories.Promotion;
using FCG.Catalog.Messages;
using FluentAssertions;
using Moq;

namespace FCG.Catalog.UnitTests.Application.UseCases.Promotions
{
    public class UpdatePromotionUseCaseTests
    {
        [Fact]
        public async Task Handle_ShouldUpdatePromotionSuccessfully_WhenValidRequest()
        {
            // Arrange
            var promotionId = Guid.NewGuid();
            var game = new GameBuilder().Build();
            var existingPromotion = Promotion.Create(game.Id, Discount.Create(10m), DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));

            var request = new UpdatePromotionInput
            {
                Id = promotionId,
                GameId = game.Id,
                DiscountPercentage = 20m,
                StartDate = DateTime.UtcNow.AddDays(2),
                EndDate = DateTime.UtcNow.AddDays(5)
            };

            var readOnlyRepoMock = new Mock<IReadOnlyPromotionRepository>();
            readOnlyRepoMock.Setup(repo => repo.GetByIdAsync(promotionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingPromotion);
            readOnlyRepoMock.Setup(repo => repo.ExistsActivePromotionForGameAsync(game.Id, request.StartDate, request.EndDate, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var writeOnlyRepoMock = new Mock<IWriteOnlyPromotionRepository>();
            writeOnlyRepoMock.Setup(repo => repo.UpdateAsync(existingPromotion, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var useCase = new UpdatePromotionUseCase(
                readOnlyRepoMock.Object,
                writeOnlyRepoMock.Object,
                unitOfWorkMock.Object);

            // Act
            var result = await useCase.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(existingPromotion.Id);
            result.GameId.Should().Be(request.GameId);
            result.Discount.Should().Be(request.DiscountPercentage);
            result.StartDate.Should().Be(request.StartDate);
            result.EndDate.Should().Be(request.EndDate);

            writeOnlyRepoMock.Verify(repo => repo.UpdateAsync(existingPromotion, It.IsAny<CancellationToken>()), Times.Once);
            unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenPromotionNotFound()
        {
            // Arrange
            var promotionId = Guid.NewGuid();
            var request = new UpdatePromotionInput
            {
                Id = promotionId,
                GameId = Guid.NewGuid(),
                DiscountPercentage = 15m,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(10)
            };

            var readOnlyRepoMock = new Mock<IReadOnlyPromotionRepository>();
            readOnlyRepoMock.Setup(repo => repo.GetByIdAsync(promotionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Promotion?)null);

            var writeOnlyRepoMock = new Mock<IWriteOnlyPromotionRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var useCase = new UpdatePromotionUseCase(
                readOnlyRepoMock.Object,
                writeOnlyRepoMock.Object,
                unitOfWorkMock.Object);

            // Act
            var act = () => useCase.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>().WithMessage("Promotion not found.");
        }

        [Fact]
        public async Task Handle_ShouldThrowDomainException_WhenActivePromotionExistsForDifferentGame()
        {
            // Arrange
            var promotionId = Guid.NewGuid();
            var game = new GameBuilder().Build();
            var existingPromotion = Promotion.Create(game.Id, Discount.Create(10m), DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));

            var request = new UpdatePromotionInput
            {
                Id = promotionId,
                GameId = Guid.NewGuid(), // Different game
                DiscountPercentage = 20m,
                StartDate = DateTime.UtcNow.AddDays(2),
                EndDate = DateTime.UtcNow.AddDays(5)
            };

            var readOnlyRepoMock = new Mock<IReadOnlyPromotionRepository>();
            readOnlyRepoMock.Setup(repo => repo.GetByIdAsync(promotionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingPromotion);
            readOnlyRepoMock.Setup(repo => repo.ExistsActivePromotionForGameAsync(request.GameId, request.StartDate, request.EndDate, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var writeOnlyRepoMock = new Mock<IWriteOnlyPromotionRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var useCase = new UpdatePromotionUseCase(
                readOnlyRepoMock.Object,
                writeOnlyRepoMock.Object,
                unitOfWorkMock.Object);

            // Act
            var act = () => useCase.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DomainException>().WithMessage("An active promotion already exists for this game in the specified period.");
        }

        [Fact]
        public async Task Handle_ShouldAllowUpdate_WhenActivePromotionExistsForSameGame()
        {
            // Arrange
            var promotionId = Guid.NewGuid();
            var game = new GameBuilder().Build();
            var existingPromotion = Promotion.Create(game.Id, Discount.Create(10m), DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));

            var request = new UpdatePromotionInput
            {
                Id = promotionId,
                GameId = game.Id, // Same game
                DiscountPercentage = 20m,
                StartDate = DateTime.UtcNow.AddDays(2),
                EndDate = DateTime.UtcNow.AddDays(5)
            };

            var readOnlyRepoMock = new Mock<IReadOnlyPromotionRepository>();
            readOnlyRepoMock.Setup(repo => repo.GetByIdAsync(promotionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingPromotion);
            readOnlyRepoMock.Setup(repo => repo.ExistsActivePromotionForGameAsync(game.Id, request.StartDate, request.EndDate, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true); // Exists but for same game

            var writeOnlyRepoMock = new Mock<IWriteOnlyPromotionRepository>();
            writeOnlyRepoMock.Setup(repo => repo.UpdateAsync(existingPromotion, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var useCase = new UpdatePromotionUseCase(
                readOnlyRepoMock.Object,
                writeOnlyRepoMock.Object,
                unitOfWorkMock.Object);

            // Act
            var result = await useCase.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            writeOnlyRepoMock.Verify(repo => repo.UpdateAsync(existingPromotion, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowDomainException_WhenDiscountPercentageIsInvalid()
        {
            // Arrange
            var promotionId = Guid.NewGuid();
            var game = new GameBuilder().Build();
            var existingPromotion = Promotion.Create(game.Id, Discount.Create(10m), DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));

            var request = new UpdatePromotionInput
            {
                Id = promotionId,
                GameId = game.Id,
                DiscountPercentage = -5m, // Invalid discount
                StartDate = DateTime.UtcNow.AddDays(2),
                EndDate = DateTime.UtcNow.AddDays(5)
            };

            var readOnlyRepoMock = new Mock<IReadOnlyPromotionRepository>();
            readOnlyRepoMock.Setup(repo => repo.GetByIdAsync(promotionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingPromotion);

            var writeOnlyRepoMock = new Mock<IWriteOnlyPromotionRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var useCase = new UpdatePromotionUseCase(
                readOnlyRepoMock.Object,
                writeOnlyRepoMock.Object,
                unitOfWorkMock.Object);

            // Act
            var act = () => useCase.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DomainException>().WithMessage(ResourceMessages.DiscountMustBeBetweenZeroAndHundred);
        }

        [Fact]
        public async Task Handle_ShouldThrowDomainException_WhenEndDateIsBeforeStartDate()
        {
            // Arrange
            var promotionId = Guid.NewGuid();
            var game = new GameBuilder().Build();
            var existingPromotion = Promotion.Create(game.Id, Discount.Create(10m), DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));

            var request = new UpdatePromotionInput
            {
                Id = promotionId,
                GameId = game.Id,
                DiscountPercentage = 20m,
                StartDate = DateTime.UtcNow.AddDays(5),
                EndDate = DateTime.UtcNow.AddDays(2) // End before start
            };

            var readOnlyRepoMock = new Mock<IReadOnlyPromotionRepository>();
            readOnlyRepoMock.Setup(repo => repo.GetByIdAsync(promotionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingPromotion);
            readOnlyRepoMock.Setup(repo => repo.ExistsActivePromotionForGameAsync(game.Id, request.StartDate, request.EndDate, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var writeOnlyRepoMock = new Mock<IWriteOnlyPromotionRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var useCase = new UpdatePromotionUseCase(
                readOnlyRepoMock.Object,
                writeOnlyRepoMock.Object,
                unitOfWorkMock.Object);

            // Act
            var act = () => useCase.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DomainException>().WithMessage(ResourceMessages.PromotionEndDateMustBeAfterStartDate);
        }

        [Fact]
        public async Task Handle_ShouldThrowDomainException_WhenGameIdIsEmpty()
        {
            // Arrange
            var promotionId = Guid.NewGuid();
            var game = new GameBuilder().Build();
            var existingPromotion = Promotion.Create(game.Id, Discount.Create(10m), DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));

            var request = new UpdatePromotionInput
            {
                Id = promotionId,
                GameId = Guid.Empty, // Empty GameId
                DiscountPercentage = 20m,
                StartDate = DateTime.UtcNow.AddDays(2),
                EndDate = DateTime.UtcNow.AddDays(5)
            };

            var readOnlyRepoMock = new Mock<IReadOnlyPromotionRepository>();
            readOnlyRepoMock.Setup(repo => repo.GetByIdAsync(promotionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingPromotion);
            readOnlyRepoMock.Setup(repo => repo.ExistsActivePromotionForGameAsync(Guid.Empty, request.StartDate, request.EndDate, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var writeOnlyRepoMock = new Mock<IWriteOnlyPromotionRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var useCase = new UpdatePromotionUseCase(
                readOnlyRepoMock.Object,
                writeOnlyRepoMock.Object,
                unitOfWorkMock.Object);

            // Act
            var act = () => useCase.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DomainException>().WithMessage(ResourceMessages.GameNotFound);
        }
    }
}