using FCG.Catalog.Application.UseCases.Promotion.Delete;
using FCG.Catalog.CommomTestUtilities.Builders.Games;
using FCG.Catalog.CommomTestUtilities.Builders.Promotions;
using FCG.Catalog.Domain.Abstractions;
using FCG.Catalog.Domain.Catalog.Entities.Promotions;
using FCG.Catalog.Domain.Catalog.ValueObjects;
using FCG.Catalog.Domain.Exception;
using FCG.Catalog.Domain.Repositories.Promotion;
using FluentAssertions;
using Moq;

namespace FCG.Catalog.UnitTests.Application.UseCases.Promotions
{
    public class DeletePromotionUseCaseTests
    {
        [Fact]
        public async Task Handle_ShouldDeletePromotionSuccessfully_WhenPromotionExists()
        {
            // Arrange
            var promotionId = Guid.NewGuid();
            var game = new GameBuilder().Build();
            var existingPromotion = Promotion.Create(game.Id, Discount.Create(10m), DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));

            var request = new DeletePromotionInput { Id = promotionId };

            var readOnlyRepoMock = new Mock<IReadOnlyPromotionRepository>();
            readOnlyRepoMock.Setup(repo => repo.GetByIdAsync(promotionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingPromotion);

            var writeOnlyRepoMock = new Mock<IWriteOnlyPromotionRepository>();
            writeOnlyRepoMock.Setup(repo => repo.DeleteAsync(existingPromotion, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var useCase = new DeletePromotionUseCase(
                readOnlyRepoMock.Object,
                writeOnlyRepoMock.Object,
                unitOfWorkMock.Object);

            // Act
            var result = await useCase.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(promotionId);

            writeOnlyRepoMock.Verify(repo => repo.DeleteAsync(existingPromotion, It.IsAny<CancellationToken>()), Times.Once);
            unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenPromotionNotFound()
        {
            // Arrange
            var promotionId = Guid.NewGuid();
            var request = new DeletePromotionInput { Id = promotionId };

            var readOnlyRepoMock = new Mock<IReadOnlyPromotionRepository>();
            readOnlyRepoMock.Setup(repo => repo.GetByIdAsync(promotionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Promotion?)null);

            var writeOnlyRepoMock = new Mock<IWriteOnlyPromotionRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var useCase = new DeletePromotionUseCase(
                readOnlyRepoMock.Object,
                writeOnlyRepoMock.Object,
                unitOfWorkMock.Object);

            // Act
            var act = () => useCase.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>().WithMessage("Promotion not found.");
        }

        [Fact]
        public async Task Handle_ShouldCallRepositoriesInCorrectOrder_WhenDeletingPromotion()
        {
            // Arrange
            var promotionId = Guid.NewGuid();
            var game = new GameBuilder().Build();
            var existingPromotion = Promotion.Create(game.Id, Discount.Create(10m), DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));

            var request = new DeletePromotionInput { Id = promotionId };
            var callOrder = new List<string>();

            var readOnlyRepoMock = new Mock<IReadOnlyPromotionRepository>();
            readOnlyRepoMock.Setup(repo => repo.GetByIdAsync(promotionId, It.IsAny<CancellationToken>()))
                .Callback(() => callOrder.Add("GetByIdAsync"))
                .ReturnsAsync(existingPromotion);

            var writeOnlyRepoMock = new Mock<IWriteOnlyPromotionRepository>();
            writeOnlyRepoMock.Setup(repo => repo.DeleteAsync(existingPromotion, It.IsAny<CancellationToken>()))
                .Callback(() => callOrder.Add("DeleteAsync"))
                .Returns(Task.CompletedTask);

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Callback(() => callOrder.Add("SaveChangesAsync"))
                .ReturnsAsync(1);

            var useCase = new DeletePromotionUseCase(
                readOnlyRepoMock.Object,
                writeOnlyRepoMock.Object,
                unitOfWorkMock.Object);

            // Act
            await useCase.Handle(request, CancellationToken.None);

            // Assert
            callOrder.Should().HaveCount(3);
            callOrder[0].Should().Be("GetByIdAsync");
            callOrder[1].Should().Be("DeleteAsync");
            callOrder[2].Should().Be("SaveChangesAsync");
        }
    }
}