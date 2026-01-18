using FCG.Catalog.Application.UseCases.Games.ProcessPurchase;
using FCG.Catalog.CommomTestUtilities.Builders;
using FCG.Catalog.CommomTestUtilities.Builders.Games;
using FCG.Catalog.CommomTestUtilities.Builders.Games.Repositories;
using FCG.Catalog.CommomTestUtilities.Builders.Libraries;
using FCG.Catalog.CommomTestUtilities.Builders.Libraries.Repositories;
using FCG.Catalog.CommomTestUtilities.Builders.LibraryGames.Repositories;
using FCG.Catalog.CommomTestUtilities.Builders.Promotions;
using FCG.Catalog.CommomTestUtilities.Builders.Promotions.Repositories;
using FCG.Catalog.CommomTestUtilities.Builders.Users;
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

            ReadOnlyGameRepositoryBuilder.Reset();
            ReadOnlyLibraryRepositoryBuilder.Reset();
            ReadOnlyLibraryGameRepositoryBuilder.Reset();
            ReadOnlyPromotionRepositoryBuilder.Reset();
            ReadOnlyPurchaseTransactionRepositoryBuilder.Reset();
            WriteOnlyPurchaseTransactionRepositoryBuilder.Reset();
            CatalogLoggedUserBuilder.Reset();
            UnitOfWorkBuilder.Reset();
        }

        [Fact]
        public async Task Handle_ShouldThrowUnauthorizedException_WhenUserNotAuthenticated()
        {
            var input = new PurchaseGameInput(Guid.NewGuid());

            CatalogLoggedUserBuilder.SetupGetLoggedUserAsync(null);

            var useCase = CreateUseCase();

            Func<Task> act = async () => await useCase.Handle(input, CancellationToken.None);

            await act.Should().ThrowAsync<UnauthorizedException>()
                .WithMessage("User not authenticated.");
        }

        [Fact]
        public async Task Handle_ShouldThrowUnauthorizedException_WhenUserIdIsEmpty()
        {
            var loggedUser = _loggedUserBuilder.BuildWithId(Guid.Empty);
            var input = new PurchaseGameInput(Guid.NewGuid());

            CatalogLoggedUserBuilder.SetupGetLoggedUserAsync(loggedUser);

            var useCase = CreateUseCase();

            Func<Task> act = async () => await useCase.Handle(input, CancellationToken.None);

            await act.Should().ThrowAsync<UnauthorizedException>();
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenGameNotFound()
        {
            var userId = Guid.NewGuid();
            var gameId = Guid.NewGuid();

            var loggedUser = _loggedUserBuilder.BuildWithId(userId);
            var input = new PurchaseGameInput(gameId);

            CatalogLoggedUserBuilder.SetupGetLoggedUserAsync(loggedUser);
            ReadOnlyGameRepositoryBuilder.SetupGetByIdActiveAsync(gameId, null);

            var useCase = CreateUseCase();

            Func<Task> act = async () => await useCase.Handle(input, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenLibraryNotFound()
        {
            var userId = Guid.NewGuid();
            var gameId = Guid.NewGuid();

            var loggedUser = _loggedUserBuilder.BuildWithId(userId);
            var game = _gameBuilder.BuildWithId(gameId);
            var input = new PurchaseGameInput(gameId);

            CatalogLoggedUserBuilder.SetupGetLoggedUserAsync(loggedUser);
            ReadOnlyGameRepositoryBuilder.SetupGetByIdActiveAsync(gameId, game);
            ReadOnlyLibraryRepositoryBuilder.SetupGetByUserIdAsync(userId, null);

            var useCase = CreateUseCase();

            Func<Task> act = async () => await useCase.Handle(input, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }


        private PurchaseGameUseCase CreateUseCase()
        {
            return new PurchaseGameUseCase(
                ReadOnlyGameRepositoryBuilder.Build(),
                ReadOnlyLibraryGameRepositoryBuilder.Build(),
                ReadOnlyPromotionRepositoryBuilder.Build(),
                ReadOnlyLibraryRepositoryBuilder.Build(),
                ReadOnlyPurchaseTransactionRepositoryBuilder.Build(),
                WriteOnlyPurchaseTransactionRepositoryBuilder.Build(),
                CachingBuilder.Build(),
                CatalogLoggedUserBuilder.Build(),
                UnitOfWorkBuilder.Build(),
                _mediatorMock.Object
            );
        }
    }
}
