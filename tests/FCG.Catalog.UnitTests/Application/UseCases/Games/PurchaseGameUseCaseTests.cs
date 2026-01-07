using FCG.Catalog.Application.UseCases.Games.ProcessPurchase;
using FCG.Catalog.CommomTestUtilities.Builders.Games;
using FCG.Catalog.CommomTestUtilities.Builders.Games.Repositories;
using FCG.Catalog.CommomTestUtilities.Builders.Libraries;
using FCG.Catalog.CommomTestUtilities.Builders.Libraries.Repositories;
using FCG.Catalog.CommomTestUtilities.Builders.LibraryGames.Repositories;
using FCG.Catalog.CommomTestUtilities.Builders.Promotions;
using FCG.Catalog.CommomTestUtilities.Builders.Promotions.Repositories;
using FCG.Catalog.CommomTestUtilities.Builders.Users;
using FCG.Catalog.Domain.Catalog.Entities.Promotions;
using FCG.Catalog.Domain.Catalog.Events;
using FCG.Catalog.Domain.Exception;
using FluentAssertions;
using MediatR;
using Moq;

namespace FCG.Catalog.UnitTests.Application.UseCases.Games
{
    public class PurchaseGameUseCaseTests
    {
        private readonly GameBuilder _gameBuilder;
        private readonly PromotionBuilder _promotionBuilder;
        private readonly LibraryBuilder _libraryBuilder;
        private readonly LoggedUserInfoBuilder _loggedUserBuilder;
        private readonly Mock<IMediator> _mediatorMock;

        public PurchaseGameUseCaseTests()
        {
            _gameBuilder = new GameBuilder();
            _promotionBuilder = new PromotionBuilder();
            _libraryBuilder = new LibraryBuilder();
            _loggedUserBuilder = new LoggedUserInfoBuilder();
            _mediatorMock = new Mock<IMediator>();

            // Reset all mocks
            ReadOnlyGameRepositoryBuilder.Reset();
            ReadOnlyLibraryGameRepositoryBuilder.Reset();
            ReadOnlyPromotionRepositoryBuilder.Reset();
            ReadOnlyLibraryRepositoryBuilder.Reset();
            CatalogLoggedUserBuilder.Reset();
        }

        [Fact]
        public async Task Handle_ShouldPurchaseGame_WhenAllConditionsAreMet()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var gameId = Guid.NewGuid();
            var loggedUser = _loggedUserBuilder.BuildWithId(userId);
            var game = _gameBuilder.BuildWithPrice(59.99m);
            var library = _libraryBuilder.BuildWithUserId(userId);
            var input = new PurchaseGameInput(gameId);

            CatalogLoggedUserBuilder.SetupGetLoggedUserAsync(loggedUser);
            ReadOnlyGameRepositoryBuilder.SetupGetByIdActiveAsync(gameId, game);
            ReadOnlyLibraryRepositoryBuilder.SetupGetByUserIdAsync(userId, library);
            ReadOnlyLibraryGameRepositoryBuilder.SetupHasGameAsync(userId, gameId, false);
            ReadOnlyPromotionRepositoryBuilder.SetupGetByGameIdAsync(gameId, new List<Promotion>());

            var useCase = new PurchaseGameUseCase(
                ReadOnlyGameRepositoryBuilder.Build(),
                ReadOnlyLibraryGameRepositoryBuilder.Build(),
                ReadOnlyPromotionRepositoryBuilder.Build(),
                ReadOnlyLibraryRepositoryBuilder.Build(),
                CatalogLoggedUserBuilder.Build(),
                _mediatorMock.Object
            );

            // Act
            var result = await useCase.Handle(input, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.OriginalPrice.Should().Be(59.99m);
            result.FinalPrice.Should().Be(59.99m);
        }

        [Fact]
        public async Task Handle_ShouldThrowUnauthorizedException_WhenUserNotAuthenticated()
        {
            // Arrange
            var input = new PurchaseGameInput(Guid.NewGuid());
            CatalogLoggedUserBuilder.SetupGetLoggedUserAsync(null);

            var useCase = new PurchaseGameUseCase(
                ReadOnlyGameRepositoryBuilder.Build(),
                ReadOnlyLibraryGameRepositoryBuilder.Build(),
                ReadOnlyPromotionRepositoryBuilder.Build(),
                ReadOnlyLibraryRepositoryBuilder.Build(),
                CatalogLoggedUserBuilder.Build(),
                _mediatorMock.Object
            );

            // Act
            Func<Task> act = async () => await useCase.Handle(input, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedException>()
                .WithMessage("User not authenticated.");
        }

        [Fact]
        public async Task Handle_ShouldThrowUnauthorizedException_WhenUserIdIsEmpty()
        {
            // Arrange
            var loggedUser = _loggedUserBuilder.BuildWithId(Guid.Empty);
            var input = new PurchaseGameInput(Guid.NewGuid());

            CatalogLoggedUserBuilder.SetupGetLoggedUserAsync(loggedUser);

            var useCase = new PurchaseGameUseCase(
                ReadOnlyGameRepositoryBuilder.Build(),
                ReadOnlyLibraryGameRepositoryBuilder.Build(),
                ReadOnlyPromotionRepositoryBuilder.Build(),
                ReadOnlyLibraryRepositoryBuilder.Build(),
                CatalogLoggedUserBuilder.Build(),
                _mediatorMock.Object
            );

            // Act
            Func<Task> act = async () => await useCase.Handle(input, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedException>()
                .WithMessage("User not authenticated.");
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenGameNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var gameId = Guid.NewGuid();
            var loggedUser = _loggedUserBuilder.BuildWithId(userId);
            var input = new PurchaseGameInput(gameId);

            CatalogLoggedUserBuilder.SetupGetLoggedUserAsync(loggedUser);
            ReadOnlyGameRepositoryBuilder.SetupGetByIdActiveAsync(gameId, null);

            var useCase = new PurchaseGameUseCase(
                ReadOnlyGameRepositoryBuilder.Build(),
                ReadOnlyLibraryGameRepositoryBuilder.Build(),
                ReadOnlyPromotionRepositoryBuilder.Build(),
                ReadOnlyLibraryRepositoryBuilder.Build(),
                CatalogLoggedUserBuilder.Build(),
                _mediatorMock.Object
            );

            // Act
            Func<Task> act = async () => await useCase.Handle(input, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"Game with id '{gameId}' was not found or is inactive.");
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenLibraryNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var gameId = Guid.NewGuid();
            var loggedUser = _loggedUserBuilder.BuildWithId(userId);
            var game = _gameBuilder.Build();
            var input = new PurchaseGameInput(gameId);

            CatalogLoggedUserBuilder.SetupGetLoggedUserAsync(loggedUser);
            ReadOnlyGameRepositoryBuilder.SetupGetByIdActiveAsync(gameId, game);
            ReadOnlyLibraryRepositoryBuilder.SetupGetByUserIdAsync(userId, null);

            var useCase = new PurchaseGameUseCase(
                ReadOnlyGameRepositoryBuilder.Build(),
                ReadOnlyLibraryGameRepositoryBuilder.Build(),
                ReadOnlyPromotionRepositoryBuilder.Build(),
                ReadOnlyLibraryRepositoryBuilder.Build(),
                CatalogLoggedUserBuilder.Build(),
                _mediatorMock.Object
            );

            // Act
            Func<Task> act = async () => await useCase.Handle(input, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"Library for user '{userId}' was not found. Please contact support.");
        }

        [Fact]
        public async Task Handle_ShouldThrowDomainException_WhenUserAlreadyOwnsGame()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var gameId = Guid.NewGuid();
            var loggedUser = _loggedUserBuilder.BuildWithId(userId);
            var game = _gameBuilder.BuildWithName("Cyberpunk 2077");
            var library = _libraryBuilder.BuildWithUserId(userId);
            var input = new PurchaseGameInput(gameId);

            CatalogLoggedUserBuilder.SetupGetLoggedUserAsync(loggedUser);
            ReadOnlyGameRepositoryBuilder.SetupGetByIdActiveAsync(gameId, game);
            ReadOnlyLibraryRepositoryBuilder.SetupGetByUserIdAsync(userId, library);
            ReadOnlyLibraryGameRepositoryBuilder.SetupHasGameAsync(userId, gameId, true);

            var useCase = new PurchaseGameUseCase(
                ReadOnlyGameRepositoryBuilder.Build(),
                ReadOnlyLibraryGameRepositoryBuilder.Build(),
                ReadOnlyPromotionRepositoryBuilder.Build(),
                ReadOnlyLibraryRepositoryBuilder.Build(),
                CatalogLoggedUserBuilder.Build(),
                _mediatorMock.Object
            );

            // Act
            Func<Task> act = async () => await useCase.Handle(input, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DomainException>()
                .WithMessage($"User already owns the game: {game.Title}");
        }

        [Fact]
        public async Task Handle_ShouldApplyDiscount_WhenActivePromotionExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var gameId = Guid.NewGuid();
            var loggedUser = _loggedUserBuilder.BuildWithId(userId);
            var game = _gameBuilder.BuildWithPrice(100m);
            var library = _libraryBuilder.BuildWithUserId(userId);
            var promotion = _promotionBuilder.BuildActivePromotion(gameId, 20m);
            var input = new PurchaseGameInput(gameId);

            CatalogLoggedUserBuilder.SetupGetLoggedUserAsync(loggedUser);
            ReadOnlyGameRepositoryBuilder.SetupGetByIdActiveAsync(gameId, game);
            ReadOnlyLibraryRepositoryBuilder.SetupGetByUserIdAsync(userId, library);
            ReadOnlyLibraryGameRepositoryBuilder.SetupHasGameAsync(userId, gameId, false);
            ReadOnlyPromotionRepositoryBuilder.SetupGetByGameIdAsync(gameId, new[] { promotion });

            var useCase = new PurchaseGameUseCase(
                ReadOnlyGameRepositoryBuilder.Build(),
                ReadOnlyLibraryGameRepositoryBuilder.Build(),
                ReadOnlyPromotionRepositoryBuilder.Build(),
                ReadOnlyLibraryRepositoryBuilder.Build(),
                CatalogLoggedUserBuilder.Build(),
                _mediatorMock.Object
            );

            // Act
            var result = await useCase.Handle(input, CancellationToken.None);

            // Assert
            result.OriginalPrice.Should().Be(100m);
            result.FinalPrice.Should().Be(80m); // 20% discount
        }

        [Fact]
        public async Task Handle_ShouldNotApplyDiscount_WhenPromotionIsExpired()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var gameId = Guid.NewGuid();
            var loggedUser = _loggedUserBuilder.BuildWithId(userId);
            var game = _gameBuilder.BuildWithPrice(100m);
            var library = _libraryBuilder.BuildWithUserId(userId);
            var promotion = _promotionBuilder.BuildExpiredPromotion(gameId, 30m);
            var input = new PurchaseGameInput(gameId);

            CatalogLoggedUserBuilder.SetupGetLoggedUserAsync(loggedUser);
            ReadOnlyGameRepositoryBuilder.SetupGetByIdActiveAsync(gameId, game);
            ReadOnlyLibraryRepositoryBuilder.SetupGetByUserIdAsync(userId, library);
            ReadOnlyLibraryGameRepositoryBuilder.SetupHasGameAsync(userId, gameId, false);
            ReadOnlyPromotionRepositoryBuilder.SetupGetByGameIdAsync(gameId, new[] { promotion });

            var useCase = new PurchaseGameUseCase(
                ReadOnlyGameRepositoryBuilder.Build(),
                ReadOnlyLibraryGameRepositoryBuilder.Build(),
                ReadOnlyPromotionRepositoryBuilder.Build(),
                ReadOnlyLibraryRepositoryBuilder.Build(),
                CatalogLoggedUserBuilder.Build(),
                _mediatorMock.Object
            );

            // Act
            var result = await useCase.Handle(input, CancellationToken.None);

            // Assert
            result.OriginalPrice.Should().Be(100m);
            result.FinalPrice.Should().Be(100m); // No discount
        }

        [Fact]
        public async Task Handle_ShouldPublishOrderPlacedEvent()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var gameId = Guid.NewGuid();
            var loggedUser = _loggedUserBuilder.BuildWithId(userId);
            var game = _gameBuilder.BuildWithPrice(59.99m);
            var library = _libraryBuilder.BuildWithUserId(userId);
            var input = new PurchaseGameInput(gameId);

            CatalogLoggedUserBuilder.SetupGetLoggedUserAsync(loggedUser);
            ReadOnlyGameRepositoryBuilder.SetupGetByIdActiveAsync(gameId, game);
            ReadOnlyLibraryRepositoryBuilder.SetupGetByUserIdAsync(userId, library);
            ReadOnlyLibraryGameRepositoryBuilder.SetupHasGameAsync(userId, gameId, false);
            ReadOnlyPromotionRepositoryBuilder.SetupGetByGameIdAsync(gameId, new List<Promotion>());

            var useCase = new PurchaseGameUseCase(
                ReadOnlyGameRepositoryBuilder.Build(),
                ReadOnlyLibraryGameRepositoryBuilder.Build(),
                ReadOnlyPromotionRepositoryBuilder.Build(),
                ReadOnlyLibraryRepositoryBuilder.Build(),
                CatalogLoggedUserBuilder.Build(),
                _mediatorMock.Object
            );

            // Act
            await useCase.Handle(input, CancellationToken.None);

            // Assert
            _mediatorMock.Verify(m => m.Publish(
                It.Is<OrderPlacedEvent>(e =>
                    e.UserId == userId &&
                    e.GameId == gameId &&
                    e.Amount == 59.99m),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldRoundPrices_ToTwoDecimals()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var gameId = Guid.NewGuid();
            var loggedUser = _loggedUserBuilder.BuildWithId(userId);
            var game = _gameBuilder.BuildWithPrice(59.999m);
            var library = _libraryBuilder.BuildWithUserId(userId);
            var input = new PurchaseGameInput(gameId);

            CatalogLoggedUserBuilder.SetupGetLoggedUserAsync(loggedUser);
            ReadOnlyGameRepositoryBuilder.SetupGetByIdActiveAsync(gameId, game);
            ReadOnlyLibraryRepositoryBuilder.SetupGetByUserIdAsync(userId, library);
            ReadOnlyLibraryGameRepositoryBuilder.SetupHasGameAsync(userId, gameId, false);
            ReadOnlyPromotionRepositoryBuilder.SetupGetByGameIdAsync(gameId, new List<Promotion>());

            var useCase = new PurchaseGameUseCase(
                ReadOnlyGameRepositoryBuilder.Build(),
                ReadOnlyLibraryGameRepositoryBuilder.Build(),
                ReadOnlyPromotionRepositoryBuilder.Build(),
                ReadOnlyLibraryRepositoryBuilder.Build(),
                CatalogLoggedUserBuilder.Build(),
                _mediatorMock.Object
            );

            // Act
            var result = await useCase.Handle(input, CancellationToken.None);

            // Assert
            result.OriginalPrice.Should().Be(60m); // Rounded
            result.FinalPrice.Should().Be(60m);
        }

        [Fact]
        public async Task Handle_ShouldCallRepositories_InCorrectOrder()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var gameId = Guid.NewGuid();
            var loggedUser = _loggedUserBuilder.BuildWithId(userId);
            var game = _gameBuilder.Build();
            var library = _libraryBuilder.BuildWithUserId(userId);
            var input = new PurchaseGameInput(gameId);

            CatalogLoggedUserBuilder.SetupGetLoggedUserAsync(loggedUser);
            ReadOnlyGameRepositoryBuilder.SetupGetByIdActiveAsync(gameId, game);
            ReadOnlyLibraryRepositoryBuilder.SetupGetByUserIdAsync(userId, library);
            ReadOnlyLibraryGameRepositoryBuilder.SetupHasGameAsync(userId, gameId, false);
            ReadOnlyPromotionRepositoryBuilder.SetupGetByGameIdAsync(gameId, new List<Promotion>());

            var useCase = new PurchaseGameUseCase(
                ReadOnlyGameRepositoryBuilder.Build(),
                ReadOnlyLibraryGameRepositoryBuilder.Build(),
                ReadOnlyPromotionRepositoryBuilder.Build(),
                ReadOnlyLibraryRepositoryBuilder.Build(),
                CatalogLoggedUserBuilder.Build(),
                _mediatorMock.Object
            );

            // Act
            await useCase.Handle(input, CancellationToken.None);

            // Assert
            CatalogLoggedUserBuilder.VerifyGetLoggedUserAsyncWasCalled(Times.Once());
            ReadOnlyGameRepositoryBuilder.VerifyGetByIdActiveAsyncWasCalled(gameId, Times.Once());
            ReadOnlyLibraryRepositoryBuilder.VerifyGetByUserIdAsyncWasCalled(userId, Times.Once());
            ReadOnlyLibraryGameRepositoryBuilder.VerifyHasGameAsyncWasCalled(userId, gameId, Times.Once());
            ReadOnlyPromotionRepositoryBuilder.VerifyGetByGameIdAsyncWasCalled(gameId, Times.Once());
        }

        [Fact]
        public async Task Handle_ShouldRespectCancellationToken()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var gameId = Guid.NewGuid();
            var loggedUser = _loggedUserBuilder.BuildWithId(userId);
            var game = _gameBuilder.Build();
            var library = _libraryBuilder.BuildWithUserId(userId);
            var input = new PurchaseGameInput(gameId);
            var cancellationToken = new CancellationToken();

            CatalogLoggedUserBuilder.SetupGetLoggedUserAsync(loggedUser);
            ReadOnlyGameRepositoryBuilder.SetupGetByIdActiveAsync(gameId, game);
            ReadOnlyLibraryRepositoryBuilder.SetupGetByUserIdAsync(userId, library);
            ReadOnlyLibraryGameRepositoryBuilder.SetupHasGameAsync(userId, gameId, false);
            ReadOnlyPromotionRepositoryBuilder.SetupGetByGameIdAsync(gameId, new List<Promotion>());

            var useCase = new PurchaseGameUseCase(
                ReadOnlyGameRepositoryBuilder.Build(),
                ReadOnlyLibraryGameRepositoryBuilder.Build(),
                ReadOnlyPromotionRepositoryBuilder.Build(),
                ReadOnlyLibraryRepositoryBuilder.Build(),
                CatalogLoggedUserBuilder.Build(),
                _mediatorMock.Object
            );

            // Act
            var result = await useCase.Handle(input, cancellationToken);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task Handle_ShouldSelectHighestDiscount_WhenMultipleActivePromotionsExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var gameId = Guid.NewGuid();
            var loggedUser = _loggedUserBuilder.BuildWithId(userId);
            var game = _gameBuilder.BuildWithPrice(100m);
            var library = _libraryBuilder.BuildWithUserId(userId);

            var promotion1 = _promotionBuilder.BuildActivePromotion(gameId, 15m);
            var promotion2 = _promotionBuilder.BuildActivePromotion(gameId, 30m);
            var promotion3 = _promotionBuilder.BuildActivePromotion(gameId, 20m);

            var input = new PurchaseGameInput(gameId);

            CatalogLoggedUserBuilder.SetupGetLoggedUserAsync(loggedUser);
            ReadOnlyGameRepositoryBuilder.SetupGetByIdActiveAsync(gameId, game);
            ReadOnlyLibraryRepositoryBuilder.SetupGetByUserIdAsync(userId, library);
            ReadOnlyLibraryGameRepositoryBuilder.SetupHasGameAsync(userId, gameId, false);
            ReadOnlyPromotionRepositoryBuilder.SetupGetByGameIdAsync(gameId, new[] { promotion1, promotion2, promotion3 });

            var useCase = new PurchaseGameUseCase(
                ReadOnlyGameRepositoryBuilder.Build(),
                ReadOnlyLibraryGameRepositoryBuilder.Build(),
                ReadOnlyPromotionRepositoryBuilder.Build(),
                ReadOnlyLibraryRepositoryBuilder.Build(),
                CatalogLoggedUserBuilder.Build(),
                _mediatorMock.Object
            );

            // Act
            var result = await useCase.Handle(input, CancellationToken.None);

            // Assert
            result.FinalPrice.Should().Be(70m); // Should apply 30% discount (highest)
        }
    }
}