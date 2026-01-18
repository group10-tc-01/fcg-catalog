using FCG.Catalog.Application.UseCases.Promotion.Create;
using FCG.Catalog.CommomTestUtilities.Builders;
using FCG.Catalog.CommomTestUtilities.Builders.Games.Repositories;
using FCG.Catalog.CommomTestUtilities.Builders.Promotions.Repositories;
using FCG.Catalog.Domain.Abstractions;
using FCG.Catalog.Domain.Catalog.Entities.Promotions;
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
    }
}