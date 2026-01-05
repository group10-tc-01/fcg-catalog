using FCG.Catalog.Application.UseCases.Games.GetById;
using FCG.Catalog.CommomTestUtilities.Builders.Games;
using FCG.Catalog.CommomTestUtilities.Builders.Games.Repositories;
using FCG.Catalog.CommomTestUtilities.Builders.Promotions;
using FCG.Catalog.Domain.Enum;
using FCG.Catalog.Domain.Exception;
using FCG.Catalog.Messages;
using FluentAssertions;
using Moq;

namespace FCG.Catalog.UnitTests.Application.UseCases.Games
{
    public class GetGameIdUseCaseTests
    {
        private readonly GameBuilder _gameBuilder;
        private readonly PromotionBuilder _promotionBuilder;

        public GetGameIdUseCaseTests()
        {
            _gameBuilder = new GameBuilder();
            _promotionBuilder = new PromotionBuilder();
            ReadOnlyGameRepositoryBuilder.Reset();
        }

        [Fact]
        public async Task Handle_ShouldReturnGameDetails_WhenGameExists()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var game = _gameBuilder.BuildWithAllParameters(
                "The Witcher 3",
                "Epic RPG Adventure",
                59.99m,
                GameCategory.RPG
            );

            var input = new GetGameIdInput(gameId);

            ReadOnlyGameRepositoryBuilder.SetupGetByIdAsync(gameId, game);

            var useCase = new GetGameIdUseCase(ReadOnlyGameRepositoryBuilder.Build());

            // Act
            var result = await useCase.Handle(input, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Title.Should().Be("The Witcher 3");
            result.Description.Should().Be("Epic RPG Adventure");
            result.Category.Should().Be(GameCategory.RPG.ToString());
            result.OriginalPrice.Should().Be(59.99m);
            result.HasActivePromotion.Should().BeFalse();
            result.DiscountedPrice.Should().BeNull();
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenGameDoesNotExist()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var input = new GetGameIdInput(gameId);

            ReadOnlyGameRepositoryBuilder.SetupGetByIdAsync(gameId, null);

            var useCase = new GetGameIdUseCase(ReadOnlyGameRepositoryBuilder.Build());

            // Act
            Func<Task> act = async () => await useCase.Handle(input, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage(ResourceMessages.GameNotFound);
        }

        [Fact]
        public async Task Handle_ShouldCallRepository_WithCorrectId()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var game = _gameBuilder.Build();
            var input = new GetGameIdInput(gameId);

            ReadOnlyGameRepositoryBuilder.SetupGetByIdAsync(gameId, game);

            var useCase = new GetGameIdUseCase(ReadOnlyGameRepositoryBuilder.Build());

            // Act
            await useCase.Handle(input, CancellationToken.None);

            // Assert
            ReadOnlyGameRepositoryBuilder.VerifyGetByIdAsyncWasCalled(gameId, Times.Once());
        }

        [Fact]
        public async Task Handle_ShouldReturnGameWithoutDiscount_WhenNoActivePromotionExists()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var game = _gameBuilder.BuildWithPrice(100m);
            var input = new GetGameIdInput(gameId);

            ReadOnlyGameRepositoryBuilder.SetupGetByIdAsync(gameId, game);

            var useCase = new GetGameIdUseCase(ReadOnlyGameRepositoryBuilder.Build());

            // Act
            var result = await useCase.Handle(input, CancellationToken.None);

            // Assert
            result.OriginalPrice.Should().Be(100m);
            result.HasActivePromotion.Should().BeFalse();
            result.DiscountedPrice.Should().BeNull();
        }

        [Fact]
        public async Task Handle_ShouldReturnDiscountedPrice_WhenActivePromotionExists()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var game = _gameBuilder.BuildWithPromotion(
                price: 100m,
                discountPercentage: 20m
            );

            var input = new GetGameIdInput(gameId);

            ReadOnlyGameRepositoryBuilder.SetupGetByIdAsync(gameId, game);

            var useCase = new GetGameIdUseCase(ReadOnlyGameRepositoryBuilder.Build());

            // Act
            var result = await useCase.Handle(input, CancellationToken.None);

            // Assert
            result.OriginalPrice.Should().Be(100m);
            result.HasActivePromotion.Should().BeTrue();
            result.DiscountedPrice.Should().Be(80m); 
        }

        [Fact]
        public async Task Handle_ShouldCalculateCorrectDiscount_WithDifferentPercentages()
        {
            // Arrange
            var game = _gameBuilder.BuildWithPromotion(
                price: 50m,
                discountPercentage: 30m
            );

            var gameId = game.Id;

            var input = new GetGameIdInput(gameId);

            ReadOnlyGameRepositoryBuilder.SetupGetByIdAsync(gameId, game);

            var useCase = new GetGameIdUseCase(ReadOnlyGameRepositoryBuilder.Build());

            // Act
            var result = await useCase.Handle(input, CancellationToken.None);

            // Assert
            result.OriginalPrice.Should().Be(50m);
            result.DiscountedPrice.Should().Be(35m); 
            result.HasActivePromotion.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_ShouldNotApplyDiscount_WhenPromotionIsExpired()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var game = _gameBuilder.BuildWithPromotion(
                price: 100m,
                discountPercentage: 25m,
                startDate: DateTime.UtcNow.AddDays(-30),
                endDate: DateTime.UtcNow.AddDays(-1) 
            );

            var input = new GetGameIdInput(gameId);

            ReadOnlyGameRepositoryBuilder.SetupGetByIdAsync(gameId, game);

            var useCase = new GetGameIdUseCase(ReadOnlyGameRepositoryBuilder.Build());

            // Act
            var result = await useCase.Handle(input, CancellationToken.None);

            // Assert
            result.OriginalPrice.Should().Be(100m);
            result.HasActivePromotion.Should().BeFalse();
            result.DiscountedPrice.Should().BeNull();
        }

        [Fact]
        public async Task Handle_ShouldNotApplyDiscount_WhenPromotionIsInFuture()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var game = _gameBuilder.BuildWithPromotion(
                price: 100m,
                discountPercentage: 25m,
                startDate: DateTime.UtcNow.AddDays(5),
                endDate: DateTime.UtcNow.AddDays(15)
            );

            var input = new GetGameIdInput(gameId);

            ReadOnlyGameRepositoryBuilder.SetupGetByIdAsync(gameId, game);

            var useCase = new GetGameIdUseCase(ReadOnlyGameRepositoryBuilder.Build());

            // Act
            var result = await useCase.Handle(input, CancellationToken.None);

            // Assert
            result.OriginalPrice.Should().Be(100m);
            result.HasActivePromotion.Should().BeFalse();
            result.DiscountedPrice.Should().BeNull();
        }

        [Fact]
        public async Task Handle_ShouldRespectCancellationToken()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var game = _gameBuilder.Build();
            var input = new GetGameIdInput(gameId);
            var cancellationToken = new CancellationToken();

            ReadOnlyGameRepositoryBuilder.SetupGetByIdAsync(gameId, game);

            var useCase = new GetGameIdUseCase(ReadOnlyGameRepositoryBuilder.Build());

            // Act
            var result = await useCase.Handle(input, cancellationToken);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task Handle_ShouldMapAllProperties_Correctly()
        {
            // Arrange
            var game = _gameBuilder.BuildWithAllParameters(
                "Cyberpunk 2077",
                "Open-world action-adventure",
                59.99m,
                GameCategory.Action
            );

            var gameId = game.Id;

            var input = new GetGameIdInput(gameId);

            ReadOnlyGameRepositoryBuilder.SetupGetByIdAsync(gameId, game);

            var useCase = new GetGameIdUseCase(ReadOnlyGameRepositoryBuilder.Build());

            // Act
            var result = await useCase.Handle(input, CancellationToken.None);

            // Assert
            result.Title.Should().Be("Cyberpunk 2077");
            result.Description.Should().Be("Open-world action-adventure");
            result.Category.Should().Be("Action");
            result.OriginalPrice.Should().Be(59.99m);
            result.HasActivePromotion.Should().BeFalse();
            result.DiscountedPrice.Should().BeNull();
        }

        [Fact]
        public async Task Handle_ShouldReturnCorrectPrice_WithHighDiscountPercentage()
        {
            // Arrange
            var game = _gameBuilder.BuildWithPromotion(
                price: 200m,
                discountPercentage: 75m // 75% de desconto
            );

            var gameId = game.Id;

            var input = new GetGameIdInput(gameId);

            ReadOnlyGameRepositoryBuilder.SetupGetByIdAsync(gameId, game);

            var useCase = new GetGameIdUseCase(ReadOnlyGameRepositoryBuilder.Build());

            // Act
            var result = await useCase.Handle(input, CancellationToken.None);

            // Assert
            result.OriginalPrice.Should().Be(200m);
            result.DiscountedPrice.Should().Be(50m); 
            result.HasActivePromotion.Should().BeTrue();
        }
    }
}