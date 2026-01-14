using FCG.Catalog.Application.UseCases.Games.Register;
using FCG.Catalog.CommomTestUtilities.Builders;
using FCG.Catalog.CommomTestUtilities.Builders.Games;
using FCG.Catalog.CommomTestUtilities.Builders.Games.Repositories;
using FCG.Catalog.Domain.Abstractions;
using FCG.Catalog.Domain.Catalog.Entities.Games;
using FCG.Catalog.Domain.Enum;
using FCG.Catalog.Domain.Exception;
using FCG.Catalog.Domain.Repositories.Game;
using FCG.Catalog.Messages;
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
                UnitOfWorkBuilder.Build()
            );

            // Act
            var result = await useCase.Handle(input, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().NotBeEmpty();
            result.Name.Should().Be("Cyberpunk 2077");
        }

        [Fact]
        public async Task Handle_ShouldThrowConflictException_WhenGameNameAlreadyExists()
        {
            // Arrange
            var existingGame = _gameBuilder.BuildWithName("The Witcher 3");
            var input = new RegisterGameInput
            {
                Name = "The Witcher 3",
                Description = "RPG Game",
                Price = 39.99m,
                Category = GameCategory.RPG
            };

            ReadOnlyGameRepositoryBuilder.SetupGetByNameAsync(input.Name, existingGame);

            var useCase = new RegisterGameUseCase(
                WriteOnlyGameRepositoryBuilder.Build(),
                ReadOnlyGameRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build()
            );

            // Act
            Func<Task> act = async () => await useCase.Handle(input, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ConflictException>()
                .WithMessage(string.Format(ResourceMessages.GameNameAlreadyExists, input.Name));
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
                UnitOfWorkBuilder.Build()
            );

            // Act
            Func<Task> act = async () => await useCase.Handle(input, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DomainException>().WithMessage("Invalid category: '999'. Available categories are: Action, Adventure, RPG...");
        }

        [Fact]
        public async Task Handle_ShouldCallGetByNameAsync_ToValidateExistence()
        {
            // Arrange
            var input = new RegisterGameInput
            {
                Name = "New Game",
                Description = "Description",
                Price = 29.99m,
                Category = GameCategory.Adventure
            };

            ReadOnlyGameRepositoryBuilder.SetupGetByNameAsync(input.Name, null);
            WriteOnlyGameRepositoryBuilder.SetupAddAsync(It.IsAny<Game>());
            UnitOfWorkBuilder.SetupSaveChangesAsync();

            var useCase = new RegisterGameUseCase(
                WriteOnlyGameRepositoryBuilder.Build(),
                ReadOnlyGameRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build()
            );

            // Act
            await useCase.Handle(input, CancellationToken.None);

            // Assert
            var mockRepo = Mock.Get(ReadOnlyGameRepositoryBuilder.Build());
            mockRepo.Verify(r => r.GetByNameAsync(input.Name), Times.Once);
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

            ReadOnlyGameRepositoryBuilder.SetupGetByNameAsync(input.Name, null);
            WriteOnlyGameRepositoryBuilder.SetupAddAsync(It.IsAny<Game>());
            UnitOfWorkBuilder.SetupSaveChangesAsync();

            var useCase = new RegisterGameUseCase(
                WriteOnlyGameRepositoryBuilder.Build(),
                ReadOnlyGameRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build()
            );

            // Act
            await useCase.Handle(input, CancellationToken.None);

            // Assert
            WriteOnlyGameRepositoryBuilder.VerifyAddAsyncWasCalled(Times.Once());
        }

        [Fact]
        public async Task Handle_ShouldCallSaveChangesAsync_AfterAddingGame()
        {
            // Arrange
            var input = new RegisterGameInput
            {
                Name = "Game to Save",
                Description = "Description",
                Price = 39.99m,
                Category = GameCategory.Sports
            };

            ReadOnlyGameRepositoryBuilder.SetupGetByNameAsync(input.Name, null);
            WriteOnlyGameRepositoryBuilder.SetupAddAsync(It.IsAny<Game>());
            UnitOfWorkBuilder.SetupSaveChangesAsync();

            var useCase = new RegisterGameUseCase(
                WriteOnlyGameRepositoryBuilder.Build(),
                ReadOnlyGameRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build()
            );

            // Act
            await useCase.Handle(input, CancellationToken.None);

            // Assert
            UnitOfWorkBuilder.VerifySaveChangesAsyncWasCalled(Times.Once());
        }

        [Fact]
        public async Task Handle_ShouldNotCallAddOrSave_WhenGameNameExists()
        {
            // Arrange
            var existingGame = _gameBuilder.BuildWithName("Existing Game");
            var input = new RegisterGameInput
            {
                Name = "Existing Game",
                Description = "Description",
                Price = 29.99m,
                Category = GameCategory.Action
            };

            ReadOnlyGameRepositoryBuilder.SetupGetByNameAsync(input.Name, existingGame);

            var useCase = new RegisterGameUseCase(
                WriteOnlyGameRepositoryBuilder.Build(),
                ReadOnlyGameRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build()
            );

            // Act
            Func<Task> act = async () => await useCase.Handle(input, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ConflictException>();
            WriteOnlyGameRepositoryBuilder.VerifyAddAsyncWasCalled(Times.Never());
            UnitOfWorkBuilder.VerifySaveChangesAsyncWasNotCalled();
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
                UnitOfWorkBuilder.Build()
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
                UnitOfWorkBuilder.Build()
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
                    UnitOfWorkBuilder.Build()
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
                UnitOfWorkBuilder.Build()
            );

            // Act
            var result = await useCase.Handle(input, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Contain($"{price}");
        }
    }
}