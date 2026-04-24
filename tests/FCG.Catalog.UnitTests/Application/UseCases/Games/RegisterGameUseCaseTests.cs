using FCG.Catalog.Application.UseCases.Games.Register;
using FCG.Catalog.CommomTestUtilities.Builders;
using FCG.Catalog.CommomTestUtilities.Builders.Games;
using FCG.Catalog.CommomTestUtilities.Builders.Games.Repositories;
using FCG.Catalog.Domain.Catalog.Entities.Games;
using FCG.Catalog.Domain.Enum;
using FCG.Catalog.Domain.Exception;
using FCG.Catalog.Domain.Models;
using FCG.Catalog.Domain.Repositories.Game;
using FluentAssertions;
using Moq;

namespace FCG.Catalog.UnitTests.Application.UseCases.Games
{
    public class RegisterGameUseCaseTests
    {
        private readonly GameBuilder _gameBuilder;

        public RegisterGameUseCaseTests()
        {
            _gameBuilder = new GameBuilder();
            ReadOnlyGameRepositoryBuilder.Reset();
            WriteOnlyGameRepositoryBuilder.Reset();
            UnitOfWorkBuilder.Reset();
        }

        [Fact]
        public async Task Handle_ShouldRegisterGame_WhenAllDataIsValid()
        {
            // Arrange
            var input = new RegisterGameInput
            {
                Name = "Cyberpunk 2077",
                Description = "Open-world action-adventure game",
                Price = 59.99m,
                Category = GameCategory.Action
            };

            ReadOnlyGameRepositoryBuilder.SetupGetByNameAsync(input.Name, null);
            WriteOnlyGameRepositoryBuilder.SetupAddAsync(It.IsAny<Game>());
            UnitOfWorkBuilder.SetupSaveChangesAsync();

            var useCase = new RegisterGameUseCase(
                WriteOnlyGameRepositoryBuilder.Build(),
                ReadOnlyGameRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build(),
                Mock.Of<IGameSearchRepository>()
            );

            // Act
            var result = await useCase.Handle(input, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().NotBeEmpty();
            result.Name.Should().Be("Cyberpunk 2077");
        }

        [Fact]
        public async Task Handle_ShouldThrowDomainException_WhenCategoryIsInvalid()
        {
            // Arrange
            var input = new RegisterGameInput
            {
                Name = "Test Game",
                Description = "Test Description",
                Price = 49.99m,
                Category = (GameCategory)999 // Invalid category
            };

            ReadOnlyGameRepositoryBuilder.SetupGetByNameAsync(input.Name, null);

            var useCase = new RegisterGameUseCase(
                WriteOnlyGameRepositoryBuilder.Build(),
                ReadOnlyGameRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build(),
                Mock.Of<IGameSearchRepository>()
            );

            // Act
            Func<Task> act = async () => await useCase.Handle(input, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DomainException>().WithMessage("Invalid category: '999'. Available categories are: Action, Adventure, RPG...");
        }

        [Fact]
        public async Task Handle_ShouldCallAddAsync_WhenGameIsValid()
        {
            // Arrange
            var input = new RegisterGameInput
            {
                Name = "Valid Game",
                Description = "Valid Description",
                Price = 19.99m,
                Category = GameCategory.Strategy
            };

            var useCase = new RegisterGameUseCase(
                WriteOnlyGameRepositoryBuilder.Build(),
                ReadOnlyGameRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build(),
                Mock.Of<IGameSearchRepository>()
            );

            ReadOnlyGameRepositoryBuilder.SetupGetByNameAsync(input.Name, null);
            WriteOnlyGameRepositoryBuilder.SetupAddAsync(It.IsAny<Game>());
            UnitOfWorkBuilder.SetupSaveChangesAsync();

            // Act
            await useCase.Handle(input, CancellationToken.None);

            // Assert
            WriteOnlyGameRepositoryBuilder.VerifyAddAsyncWasCalled(Times.Once());
        }

        [Fact]
        public async Task Handle_ShouldRespectCancellationToken()
        {
            // Arrange
            var input = new RegisterGameInput
            {
                Name = "Test Game",
                Description = "Test",
                Price = 49.99m,
                Category = GameCategory.RPG
            };
            var cancellationToken = CancellationToken.None;

            ReadOnlyGameRepositoryBuilder.SetupGetByNameAsync(input.Name, null);
            WriteOnlyGameRepositoryBuilder.SetupAddAsync(It.IsAny<Game>());
            UnitOfWorkBuilder.SetupSaveChangesAsync();

            var useCase = new RegisterGameUseCase(
                WriteOnlyGameRepositoryBuilder.Build(),
                ReadOnlyGameRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build(),
                Mock.Of<IGameSearchRepository>()
            );

            // Act
            var result = await useCase.Handle(input, cancellationToken);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task Handle_ShouldCreateGameWithCorrectProperties()
        {
            // Arrange
            var input = new RegisterGameInput
            {
                Name = "Specific Game",
                Description = "Specific Description",
                Price = 99.99m,
                Category = GameCategory.Simulation
            };

            ReadOnlyGameRepositoryBuilder.SetupGetByNameAsync(input.Name, null);
            WriteOnlyGameRepositoryBuilder.SetupAddAsync(It.IsAny<Game>());
            UnitOfWorkBuilder.SetupSaveChangesAsync();

            var useCase = new RegisterGameUseCase(
                WriteOnlyGameRepositoryBuilder.Build(),
                ReadOnlyGameRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build(),
                Mock.Of<IGameSearchRepository>()
            );

            // Act
            var result = await useCase.Handle(input, CancellationToken.None);

            // Assert
            result.Name.Should().Be("Specific Game");
            result.Id.Should().NotBeEmpty();
        }

        [Fact]
        public async Task Handle_ShouldAcceptAllValidCategories()
        {
            // Arrange & Act & Assert
            var validCategories = new[]
            {
                GameCategory.Action,
                GameCategory.Adventure,
                GameCategory.RPG,
                GameCategory.Strategy,
                GameCategory.Sports,
                GameCategory.Simulation
            };

            foreach (var category in validCategories)
            {
                ReadOnlyGameRepositoryBuilder.Reset();
                WriteOnlyGameRepositoryBuilder.Reset();
                UnitOfWorkBuilder.Reset();

                var input = new RegisterGameInput
                {
                    Name = $"Game {category}",
                    Description = "Description",
                    Price = 49.99m,
                    Category = category
                };

                ReadOnlyGameRepositoryBuilder.SetupGetByNameAsync(input.Name, null);
                WriteOnlyGameRepositoryBuilder.SetupAddAsync(It.IsAny<Game>());
                UnitOfWorkBuilder.SetupSaveChangesAsync();

                var useCase = new RegisterGameUseCase(
                    WriteOnlyGameRepositoryBuilder.Build(),
                    ReadOnlyGameRepositoryBuilder.Build(),
                    UnitOfWorkBuilder.Build(),
                    Mock.Of<IGameSearchRepository>()
                );

                var result = await useCase.Handle(input, CancellationToken.None);
                result.Should().NotBeNull();
            }
        }

        [Theory]
        [InlineData(0.01)]
        [InlineData(9.99)]
        [InlineData(59.99)]
        [InlineData(99.99)]
        [InlineData(199.99)]
        public async Task Handle_ShouldAcceptValidPrices(decimal price)
        {
            // Arrange
            var input = new RegisterGameInput
            {
                Name = $"Game Price {price}",
                Description = "Description",
                Price = price,
                Category = GameCategory.Action
            };

            ReadOnlyGameRepositoryBuilder.SetupGetByNameAsync(input.Name, null);
            WriteOnlyGameRepositoryBuilder.SetupAddAsync(It.IsAny<Game>());
            UnitOfWorkBuilder.SetupSaveChangesAsync();

            var useCase = new RegisterGameUseCase(
                WriteOnlyGameRepositoryBuilder.Build(),
                ReadOnlyGameRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build(),
                Mock.Of<IGameSearchRepository>()
            );

            // Act
            var result = await useCase.Handle(input, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Contain($"{price}");
        }

        [Fact]
        public async Task Handle_ShouldIndexGameAfterSaving_WhenGameIsRegistered()
        {
            // Arrange
            var input = new RegisterGameInput
            {
                Name = "Indexed Game",
                Description = "Indexed Description",
                Price = 79.90m,
                Category = GameCategory.Racing
            };

            var readRepoMock = new Mock<IReadOnlyGameRepository>();
            readRepoMock
                .Setup(x => x.GetByNameAsync(input.Name))
                .ReturnsAsync((Game?)null);

            var writeRepoMock = new Mock<IWriteOnlyGameRepository>();
            writeRepoMock
                .Setup(x => x.AddAsync(It.IsAny<Game>()))
                .Returns(Task.CompletedTask);

            var unitOfWorkMock = new Mock<FCG.Catalog.Domain.Abstractions.IUnitOfWork>();
            unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var searchRepositoryMock = new Mock<IGameSearchRepository>();

            var useCase = new RegisterGameUseCase(
                writeRepoMock.Object,
                readRepoMock.Object,
                unitOfWorkMock.Object,
                searchRepositoryMock.Object);

            // Act
            await useCase.Handle(input, CancellationToken.None);

            // Assert
            searchRepositoryMock.Verify(
                x => x.IndexAsync(
                    It.Is<GameSearch>(game =>
                        game.Title == input.Name &&
                        game.Description == input.Description &&
                        game.Price == input.Price &&
                        game.DiscountedPrice == input.Price &&
                        game.Category == input.Category.ToString() &&
                        game.IsActive),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
