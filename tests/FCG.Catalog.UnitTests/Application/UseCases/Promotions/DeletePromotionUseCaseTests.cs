using FCG.Catalog.Application.UseCases.Promotion.Delete;
using FCG.Catalog.CommomTestUtilities.Builders;
using FCG.Catalog.CommomTestUtilities.Builders.Promotions;
using FCG.Catalog.CommomTestUtilities.Builders.Promotions.Repositories;
using FCG.Catalog.Domain.Abstractions;
using FCG.Catalog.Domain.Exception;
using FCG.Catalog.Domain.Repositories.Promotion;
using FluentAssertions;
using Moq;

namespace FCG.Catalog.UnitTests.Application.UseCases.Promotions
{
    public class DeletePromotionUseCaseTests
    {
        private readonly PromotionBuilder _promotionBuilder;

        public DeletePromotionUseCaseTests()
        {
            _promotionBuilder = new PromotionBuilder();
            ReadOnlyPromotionRepositoryBuilder.Reset();
            WriteOnlyPromotionRepositoryBuilder.Reset();
            UnitOfWorkBuilder.Reset();
        }

        [Fact]
        public async Task Handle_ShouldDeletePromotion_WhenPromotionExists()
        {
            // Arrange
            var promotionId = Guid.NewGuid();
            var gameId = Guid.NewGuid();
            var promotion = _promotionBuilder.BuildActivePromotion(gameId, 20m);
            var input = new DeletePromotionInput { Id = promotionId };

            ReadOnlyPromotionRepositoryBuilder.SetupGetByIdAsync(promotionId, promotion);
            WriteOnlyPromotionRepositoryBuilder.SetupDeleteAsync();
            UnitOfWorkBuilder.SetupSaveChangesAsync();

            var useCase = new DeletePromotionUseCase(
                ReadOnlyPromotionRepositoryBuilder.Build(),
                WriteOnlyPromotionRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build()
            );

            // Act
            var result = await useCase.Handle(input, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(promotionId);
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenPromotionDoesNotExist()
        {
            // Arrange
            var promotionId = Guid.NewGuid();
            var input = new DeletePromotionInput { Id = promotionId };

            ReadOnlyPromotionRepositoryBuilder.SetupGetByIdAsync(promotionId, null);

            var useCase = new DeletePromotionUseCase(
                ReadOnlyPromotionRepositoryBuilder.Build(),
                WriteOnlyPromotionRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build()
            );

            // Act
            Func<Task> act = async () => await useCase.Handle(input, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("Promotion not found.");
        }

        [Fact]
        public async Task Handle_ShouldCallGetByIdAsync_WithCorrectId()
        {
            // Arrange
            var promotionId = Guid.NewGuid();
            var gameId = Guid.NewGuid();
            var promotion = _promotionBuilder.BuildActivePromotion(gameId, 15m);
            var input = new DeletePromotionInput { Id = promotionId };

            ReadOnlyPromotionRepositoryBuilder.SetupGetByIdAsync(promotionId, promotion);
            WriteOnlyPromotionRepositoryBuilder.SetupDeleteAsync();
            UnitOfWorkBuilder.SetupSaveChangesAsync();

            var useCase = new DeletePromotionUseCase(
                ReadOnlyPromotionRepositoryBuilder.Build(),
                WriteOnlyPromotionRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build()
            );

            // Act
            await useCase.Handle(input, CancellationToken.None);

            // Assert
            ReadOnlyPromotionRepositoryBuilder.VerifyGetByIdAsyncWasCalled(promotionId, Times.Once());
        }

        [Fact]
        public async Task Handle_ShouldCallDeleteAsync_WhenPromotionExists()
        {
            // Arrange
            var promotionId = Guid.NewGuid();
            var gameId = Guid.NewGuid();
            var promotion = _promotionBuilder.BuildActivePromotion(gameId, 25m);
            var input = new DeletePromotionInput { Id = promotionId };

            ReadOnlyPromotionRepositoryBuilder.SetupGetByIdAsync(promotionId, promotion);
            WriteOnlyPromotionRepositoryBuilder.SetupDeleteAsync();
            UnitOfWorkBuilder.SetupSaveChangesAsync();

            var useCase = new DeletePromotionUseCase(
                ReadOnlyPromotionRepositoryBuilder.Build(),
                WriteOnlyPromotionRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build()
            );

            // Act
            await useCase.Handle(input, CancellationToken.None);

            // Assert
            WriteOnlyPromotionRepositoryBuilder.VerifyDeleteAsyncWasCalled(Times.Once());
        }

        [Fact]
        public async Task Handle_ShouldCallSaveChangesAsync_AfterDelete()
        {
            // Arrange
            var promotionId = Guid.NewGuid();
            var gameId = Guid.NewGuid();
            var promotion = _promotionBuilder.BuildActivePromotion(gameId, 30m);
            var input = new DeletePromotionInput { Id = promotionId };

            ReadOnlyPromotionRepositoryBuilder.SetupGetByIdAsync(promotionId, promotion);
            WriteOnlyPromotionRepositoryBuilder.SetupDeleteAsync();
            UnitOfWorkBuilder.SetupSaveChangesAsync();

            var useCase = new DeletePromotionUseCase(
                ReadOnlyPromotionRepositoryBuilder.Build(),
                WriteOnlyPromotionRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build()
            );

            // Act
            await useCase.Handle(input, CancellationToken.None);

            // Assert
            UnitOfWorkBuilder.VerifySaveChangesAsyncWasCalled(Times.Once());
        }

        [Fact]
        public async Task Handle_ShouldNotCallDeleteOrSave_WhenPromotionNotFound()
        {
            // Arrange
            var promotionId = Guid.NewGuid();
            var input = new DeletePromotionInput { Id = promotionId };

            ReadOnlyPromotionRepositoryBuilder.SetupGetByIdAsync(promotionId, null);

            var useCase = new DeletePromotionUseCase(
                ReadOnlyPromotionRepositoryBuilder.Build(),
                WriteOnlyPromotionRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build()
            );

            // Act
            Func<Task> act = async () => await useCase.Handle(input, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
            WriteOnlyPromotionRepositoryBuilder.VerifyDeleteAsyncWasCalled(Times.Never());
            UnitOfWorkBuilder.VerifySaveChangesAsyncWasNotCalled();
        }

        [Fact]
        public async Task Handle_ShouldRespectCancellationToken()
        {
            // Arrange
            var promotionId = Guid.NewGuid();
            var gameId = Guid.NewGuid();
            var promotion = _promotionBuilder.BuildActivePromotion(gameId, 10m);
            var input = new DeletePromotionInput { Id = promotionId };
            var cancellationToken = new CancellationToken();

            ReadOnlyPromotionRepositoryBuilder.SetupGetByIdAsync(promotionId, promotion);
            WriteOnlyPromotionRepositoryBuilder.SetupDeleteAsync();
            UnitOfWorkBuilder.SetupSaveChangesAsync();

            var useCase = new DeletePromotionUseCase(
                ReadOnlyPromotionRepositoryBuilder.Build(),
                WriteOnlyPromotionRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build()
            );

            // Act
            var result = await useCase.Handle(input, cancellationToken);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task Handle_ShouldExecuteInCorrectOrder()
        {
            // Arrange
            var promotionId = Guid.NewGuid();
            var gameId = Guid.NewGuid();
            var promotion = _promotionBuilder.BuildActivePromotion(gameId, 20m);
            var callOrder = new List<string>();

            var input = new DeletePromotionInput { Id = promotionId };

            var mockReadRepo = new Mock<IReadOnlyPromotionRepository>();
            mockReadRepo.Setup(r => r.GetByIdAsync(promotionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(promotion)
                .Callback(() => callOrder.Add("GetById"));

            var mockWriteRepo = new Mock<IWriteOnlyPromotionRepository>();
            mockWriteRepo.Setup(r => r.DeleteAsync(promotion, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Callback(() => callOrder.Add("DeleteAsync"));

            var mockUow = new Mock<IUnitOfWork>();
            mockUow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1)
                .Callback(() => callOrder.Add("SaveChanges"));

            var useCase = new DeletePromotionUseCase(
                mockReadRepo.Object,
                mockWriteRepo.Object,
                mockUow.Object
            );

            // Act
            await useCase.Handle(input, CancellationToken.None);

            // Assert
            callOrder.Should().Equal("GetById", "DeleteAsync", "SaveChanges");
        }

        [Fact]
        public async Task Handle_ShouldReturnCorrectOutput_WithPromotionId()
        {
            // Arrange
            var promotionId = Guid.NewGuid();
            var gameId = Guid.NewGuid();
            var promotion = _promotionBuilder.BuildActivePromotion(gameId, 50m);
            var input = new DeletePromotionInput { Id = promotionId };

            ReadOnlyPromotionRepositoryBuilder.SetupGetByIdAsync(promotionId, promotion);
            WriteOnlyPromotionRepositoryBuilder.SetupDeleteAsync();
            UnitOfWorkBuilder.SetupSaveChangesAsync();

            var useCase = new DeletePromotionUseCase(
                ReadOnlyPromotionRepositoryBuilder.Build(),
                WriteOnlyPromotionRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build()
            );

            // Act
            var result = await useCase.Handle(input, CancellationToken.None);

            // Assert
            result.Id.Should().Be(promotionId);
            result.Should().BeOfType<DeletePromotionOutput>();
        }

        [Fact]
        public async Task Handle_ShouldDeleteActivePromotion()
        {
            // Arrange
            var promotionId = Guid.NewGuid();
            var gameId = Guid.NewGuid();
            var activePromotion = _promotionBuilder.BuildActivePromotion(gameId, 40m);
            var input = new DeletePromotionInput { Id = promotionId };

            ReadOnlyPromotionRepositoryBuilder.SetupGetByIdAsync(promotionId, activePromotion);
            WriteOnlyPromotionRepositoryBuilder.SetupDeleteAsync();
            UnitOfWorkBuilder.SetupSaveChangesAsync();

            var useCase = new DeletePromotionUseCase(
                ReadOnlyPromotionRepositoryBuilder.Build(),
                WriteOnlyPromotionRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build()
            );

            // Act
            var result = await useCase.Handle(input, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            WriteOnlyPromotionRepositoryBuilder.VerifyDeleteAsyncWasCalled(Times.Once());
        }

        [Fact]
        public async Task Handle_ShouldDeleteExpiredPromotion()
        {
            // Arrange
            var promotionId = Guid.NewGuid();
            var gameId = Guid.NewGuid();
            var expiredPromotion = _promotionBuilder.BuildExpiredPromotion(gameId, 35m);
            var input = new DeletePromotionInput { Id = promotionId };

            ReadOnlyPromotionRepositoryBuilder.SetupGetByIdAsync(promotionId, expiredPromotion);
            WriteOnlyPromotionRepositoryBuilder.SetupDeleteAsync();
            UnitOfWorkBuilder.SetupSaveChangesAsync();

            var useCase = new DeletePromotionUseCase(
                ReadOnlyPromotionRepositoryBuilder.Build(),
                WriteOnlyPromotionRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build()
            );

            // Act
            var result = await useCase.Handle(input, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            WriteOnlyPromotionRepositoryBuilder.VerifyDeleteAsyncWasCalled(Times.Once());
        }

        [Fact]
        public async Task Handle_ShouldDeleteFuturePromotion()
        {
            // Arrange
            var promotionId = Guid.NewGuid();
            var gameId = Guid.NewGuid();
            var futurePromotion = _promotionBuilder.BuildFuturePromotion(gameId, 45m);
            var input = new DeletePromotionInput { Id = promotionId };

            ReadOnlyPromotionRepositoryBuilder.SetupGetByIdAsync(promotionId, futurePromotion);
            WriteOnlyPromotionRepositoryBuilder.SetupDeleteAsync();
            UnitOfWorkBuilder.SetupSaveChangesAsync();

            var useCase = new DeletePromotionUseCase(
                ReadOnlyPromotionRepositoryBuilder.Build(),
                WriteOnlyPromotionRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build()
            );

            // Act
            var result = await useCase.Handle(input, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            WriteOnlyPromotionRepositoryBuilder.VerifyDeleteAsyncWasCalled(Times.Once());
        }
    }
}