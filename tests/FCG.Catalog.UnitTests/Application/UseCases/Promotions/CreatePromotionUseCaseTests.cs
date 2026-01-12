using FCG.Catalog.Application.UseCases.Promotion.Create;
using FCG.Catalog.CommomTestUtilities.Builders;
using FCG.Catalog.CommomTestUtilities.Builders.Games.Repositories;
using FCG.Catalog.CommomTestUtilities.Builders.Promotions.Repositories;
using FCG.Catalog.Domain.Abstractions;
using FCG.Catalog.Domain.Catalog.Entities.Promotions;
using FCG.Catalog.Domain.Exception;
using FCG.Catalog.Domain.Repositories.Game;
using FCG.Catalog.Domain.Repositories.Promotion;
using FluentAssertions;
using Moq;

namespace FCG.Catalog.UnitTests.Application.UseCases.Promotions
{
    public class CreatePromotionUseCaseTests
    {
        public CreatePromotionUseCaseTests()
        {
            ReadOnlyGameRepositoryBuilder.Reset();
            ReadOnlyPromotionRepositoryBuilder.Reset();
            WriteOnlyPromotionRepositoryBuilder.Reset();
            UnitOfWorkBuilder.Reset();
        }

        [Fact]
        public async Task Handle_ShouldCreatePromotion_WhenGameExistsAndNoActivePromotion()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var startDate = DateTime.UtcNow.AddDays(1);
            var endDate = DateTime.UtcNow.AddDays(10);
            var discountPercentage = 25m;

            var request = new CreatePromotionInput
            {
                GameId = gameId,
                DiscountPercentage = discountPercentage,
                StartDate = startDate,
                EndDate = endDate
            };

            ReadOnlyGameRepositoryBuilder.SetupExistsAsync(gameId, true);
            ReadOnlyPromotionRepositoryBuilder.SetupExistsActivePromotionForGameAsync(gameId, startDate, endDate, false);
            WriteOnlyPromotionRepositoryBuilder.SetupAddAsync();
            UnitOfWorkBuilder.SetupSaveChangesAsync();

            var useCase = new CreatePromotionUseCase(
                ReadOnlyGameRepositoryBuilder.Build(),
                ReadOnlyPromotionRepositoryBuilder.Build(),
                WriteOnlyPromotionRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build()
            );

            // Act
            var result = await useCase.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().NotBe(Guid.Empty);
            result.GameId.Should().Be(gameId);
            result.Discount.Should().Be(discountPercentage);
            result.StartDate.Should().Be(startDate);
            result.EndDate.Should().Be(endDate);

            ReadOnlyGameRepositoryBuilder.VerifyExistsAsyncWasCalled(gameId, Times.Once());
            ReadOnlyPromotionRepositoryBuilder.VerifyExistsActivePromotionForGameAsyncWasCalled(gameId, startDate, endDate, Times.Once());
            WriteOnlyPromotionRepositoryBuilder.VerifyAddAsyncWasCalled(Times.Once());
            UnitOfWorkBuilder.VerifySaveChangesAsyncWasCalled(Times.Once());
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenGameDoesNotExist()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var request = new CreatePromotionInput
            {
                GameId = gameId,
                DiscountPercentage = 20m,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(10)
            };

            ReadOnlyGameRepositoryBuilder.SetupExistsAsync(gameId, false);

            var useCase = new CreatePromotionUseCase(
                ReadOnlyGameRepositoryBuilder.Build(),
                ReadOnlyPromotionRepositoryBuilder.Build(),
                WriteOnlyPromotionRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build()
            );

            // Act
            Func<Task> act = async () => await useCase.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("Game not found.");

            ReadOnlyGameRepositoryBuilder.VerifyExistsAsyncWasCalled(gameId, Times.Once());
            ReadOnlyPromotionRepositoryBuilder.VerifyExistsActivePromotionForGameAsyncWasCalled(gameId, Times.Never());
            WriteOnlyPromotionRepositoryBuilder.VerifyAddAsyncWasCalled(Times.Never());
            UnitOfWorkBuilder.VerifySaveChangesAsyncWasCalled(Times.Never());
        }

        [Fact]
        public async Task Handle_ShouldThrowDomainException_WhenActivePromotionAlreadyExists()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var startDate = DateTime.UtcNow.AddDays(1);
            var endDate = DateTime.UtcNow.AddDays(10);

            var request = new CreatePromotionInput
            {
                GameId = gameId,
                DiscountPercentage = 30m,
                StartDate = startDate,
                EndDate = endDate
            };

            ReadOnlyGameRepositoryBuilder.SetupExistsAsync(gameId, true);
            ReadOnlyPromotionRepositoryBuilder.SetupExistsActivePromotionForGameAsync(gameId, startDate, endDate, true);

            var useCase = new CreatePromotionUseCase(
                ReadOnlyGameRepositoryBuilder.Build(),
                ReadOnlyPromotionRepositoryBuilder.Build(),
                WriteOnlyPromotionRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build()
            );

            // Act
            Func<Task> act = async () => await useCase.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DomainException>()
                .WithMessage("An active promotion already exists for this game in the specified period.");

            ReadOnlyGameRepositoryBuilder.VerifyExistsAsyncWasCalled(gameId, Times.Once());
            ReadOnlyPromotionRepositoryBuilder.VerifyExistsActivePromotionForGameAsyncWasCalled(gameId, startDate, endDate, Times.Once());
            WriteOnlyPromotionRepositoryBuilder.VerifyAddAsyncWasCalled(Times.Never());
            UnitOfWorkBuilder.VerifySaveChangesAsyncWasCalled(Times.Never());
        }

        [Fact]
        public async Task Handle_ShouldCallRepositoriesInCorrectOrder_WhenCreatingPromotion()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var startDate = DateTime.UtcNow.AddDays(1);
            var endDate = DateTime.UtcNow.AddDays(10);
            var callOrder = new List<string>();

            var request = new CreatePromotionInput
            {
                GameId = gameId,
                DiscountPercentage = 15m,
                StartDate = startDate,
                EndDate = endDate
            };

            ReadOnlyGameRepositoryBuilder.Reset();
            var gameRepoMock = new Mock<IReadOnlyGameRepository>();
            gameRepoMock.Setup(repo => repo.ExistsAsync(gameId, It.IsAny<CancellationToken>()))
                .Callback(() => callOrder.Add("ExistsAsync"))
                .ReturnsAsync(true);

            ReadOnlyPromotionRepositoryBuilder.Reset();
            var promoRepoMock = new Mock<IReadOnlyPromotionRepository>();
            promoRepoMock.Setup(repo => repo.ExistsActivePromotionForGameAsync(gameId, startDate, endDate, It.IsAny<CancellationToken>()))
                .Callback(() => callOrder.Add("ExistsActivePromotion"))
                .ReturnsAsync(false);

            WriteOnlyPromotionRepositoryBuilder.Reset();
            var writePromoMock = new Mock<IWriteOnlyPromotionRepository>();
            writePromoMock.Setup(repo => repo.AddAsync(It.IsAny<Promotion>(), It.IsAny<CancellationToken>()))
                .Callback(() => callOrder.Add("AddAsync"))
                .Returns(Task.CompletedTask);

            UnitOfWorkBuilder.Reset();
            var uowMock = new Mock<IUnitOfWork>();
            uowMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Callback(() => callOrder.Add("SaveChanges"))
                .ReturnsAsync(1);

            var useCase = new CreatePromotionUseCase(
                gameRepoMock.Object,
                promoRepoMock.Object,
                writePromoMock.Object,
                uowMock.Object
            );

            // Act
            await useCase.Handle(request, CancellationToken.None);

            // Assert
            callOrder.Should().HaveCount(4);
            callOrder[0].Should().Be("ExistsAsync");
            callOrder[1].Should().Be("ExistsActivePromotion");
            callOrder[2].Should().Be("AddAsync");
            callOrder[3].Should().Be("SaveChanges");
        }

        [Fact]
        public async Task Handle_ShouldCreatePromotionWithCorrectDiscount_WhenCalled()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var discountPercentage = 50m;
            var request = new CreatePromotionInput
            {
                GameId = gameId,
                DiscountPercentage = discountPercentage,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(10)
            };

            ReadOnlyGameRepositoryBuilder.SetupExistsAsync(gameId, true);
            ReadOnlyPromotionRepositoryBuilder.SetupExistsActivePromotionForGameAsync(
                gameId,
                request.StartDate,
                request.EndDate,
                false);
            WriteOnlyPromotionRepositoryBuilder.SetupAddAsync();
            UnitOfWorkBuilder.SetupSaveChangesAsync();

            var useCase = new CreatePromotionUseCase(
                ReadOnlyGameRepositoryBuilder.Build(),
                ReadOnlyPromotionRepositoryBuilder.Build(),
                WriteOnlyPromotionRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build()
            );

            // Act
            var result = await useCase.Handle(request, CancellationToken.None);

            // Assert
            result.Discount.Should().Be(discountPercentage);
        }

        [Fact]
        public async Task Handle_ShouldPersistChanges_WhenPromotionIsCreatedSuccessfully()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var request = new CreatePromotionInput
            {
                GameId = gameId,
                DiscountPercentage = 20m,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(10)
            };

            ReadOnlyGameRepositoryBuilder.SetupExistsAsync(gameId, true);
            ReadOnlyPromotionRepositoryBuilder.SetupExistsActivePromotionForGameAsync(
                gameId,
                request.StartDate,
                request.EndDate,
                false);
            WriteOnlyPromotionRepositoryBuilder.SetupAddAsync();
            UnitOfWorkBuilder.SetupSaveChangesAsync();

            var useCase = new CreatePromotionUseCase(
                ReadOnlyGameRepositoryBuilder.Build(),
                ReadOnlyPromotionRepositoryBuilder.Build(),
                WriteOnlyPromotionRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build()
            );

            // Act
            await useCase.Handle(request, CancellationToken.None);

            // Assert
            UnitOfWorkBuilder.VerifySaveChangesAsyncWasCalled(Times.Once());
        }

        [Fact]
        public async Task Handle_ShouldNotPersistChanges_WhenGameDoesNotExist()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var request = new CreatePromotionInput
            {
                GameId = gameId,
                DiscountPercentage = 25m,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(10)
            };

            ReadOnlyGameRepositoryBuilder.SetupExistsAsync(gameId, false);

            var useCase = new CreatePromotionUseCase(
                ReadOnlyGameRepositoryBuilder.Build(),
                ReadOnlyPromotionRepositoryBuilder.Build(),
                WriteOnlyPromotionRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build()
            );

            // Act
            try
            {
                await useCase.Handle(request, CancellationToken.None);
            }
            catch (NotFoundException)
            {
                // Esperado
            }

            // Assert
            UnitOfWorkBuilder.VerifySaveChangesAsyncWasCalled(Times.Never());
        }

        [Fact]
        public async Task Handle_ShouldNotPersistChanges_WhenActivePromotionExists()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var startDate = DateTime.UtcNow.AddDays(1);
            var endDate = DateTime.UtcNow.AddDays(10);

            var request = new CreatePromotionInput
            {
                GameId = gameId,
                DiscountPercentage = 30m,
                StartDate = startDate,
                EndDate = endDate
            };

            ReadOnlyGameRepositoryBuilder.SetupExistsAsync(gameId, true);
            ReadOnlyPromotionRepositoryBuilder.SetupExistsActivePromotionForGameAsync(gameId, startDate, endDate, true);

            var useCase = new CreatePromotionUseCase(
                ReadOnlyGameRepositoryBuilder.Build(),
                ReadOnlyPromotionRepositoryBuilder.Build(),
                WriteOnlyPromotionRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build()
            );

            // Act
            try
            {
                await useCase.Handle(request, CancellationToken.None);
            }
            catch (DomainException)
            {
                // Esperado
            }

            // Assert
            UnitOfWorkBuilder.VerifySaveChangesAsyncWasCalled(Times.Never());
        }

        [Theory]
        [InlineData(10)]
        [InlineData(25)]
        [InlineData(50)]
        [InlineData(75)]
        public async Task Handle_ShouldCreatePromotionWithDifferentDiscounts_WhenValid(decimal discount)
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var request = new CreatePromotionInput
            {
                GameId = gameId,
                DiscountPercentage = discount,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(10)
            };

            ReadOnlyGameRepositoryBuilder.SetupExistsAsync(gameId, true);
            ReadOnlyPromotionRepositoryBuilder.SetupExistsActivePromotionForGameAsync(
                gameId,
                request.StartDate,
                request.EndDate,
                false);
            WriteOnlyPromotionRepositoryBuilder.SetupAddAsync();
            UnitOfWorkBuilder.SetupSaveChangesAsync();

            var useCase = new CreatePromotionUseCase(
                ReadOnlyGameRepositoryBuilder.Build(),
                ReadOnlyPromotionRepositoryBuilder.Build(),
                WriteOnlyPromotionRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build()
            );

            // Act
            var result = await useCase.Handle(request, CancellationToken.None);

            // Assert
            result.Discount.Should().Be(discount);
            WriteOnlyPromotionRepositoryBuilder.VerifyAddAsyncWasCalled(Times.Once());
        }

        [Fact]
        public async Task Handle_ShouldReturnPromotionWithGeneratedId_WhenCreated()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var request = new CreatePromotionInput
            {
                GameId = gameId,
                DiscountPercentage = 15m,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(10)
            };

            ReadOnlyGameRepositoryBuilder.SetupExistsAsync(gameId, true);
            ReadOnlyPromotionRepositoryBuilder.SetupExistsActivePromotionForGameAsync(
                gameId,
                request.StartDate,
                request.EndDate,
                false);
            WriteOnlyPromotionRepositoryBuilder.SetupAddAsync();
            UnitOfWorkBuilder.SetupSaveChangesAsync();

            var useCase = new CreatePromotionUseCase(
                ReadOnlyGameRepositoryBuilder.Build(),
                ReadOnlyPromotionRepositoryBuilder.Build(),
                WriteOnlyPromotionRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build()
            );

            // Act
            var result = await useCase.Handle(request, CancellationToken.None);

            // Assert
            result.Id.Should().NotBe(Guid.Empty);
        }

        [Fact]
        public async Task Handle_ShouldCheckGameExistenceBeforeCheckingPromotion_WhenCalled()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var request = new CreatePromotionInput
            {
                GameId = gameId,
                DiscountPercentage = 20m,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(10)
            };

            ReadOnlyGameRepositoryBuilder.SetupExistsAsync(gameId, false);

            var useCase = new CreatePromotionUseCase(
                ReadOnlyGameRepositoryBuilder.Build(),
                ReadOnlyPromotionRepositoryBuilder.Build(),
                WriteOnlyPromotionRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build()
            );

            // Act
            try
            {
                await useCase.Handle(request, CancellationToken.None);
            }
            catch (NotFoundException)
            {
                // Esperado
            }

            // Assert
            ReadOnlyGameRepositoryBuilder.VerifyExistsAsyncWasCalled(gameId, Times.Once());
            ReadOnlyPromotionRepositoryBuilder.VerifyExistsActivePromotionForGameAsyncWasCalled(gameId, Times.Never());
        }
    }
}