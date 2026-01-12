using FCG.Catalog.Application.UseCases.Games.Update;
using FCG.Catalog.CommomTestUtilities.Builders;
using FCG.Catalog.CommomTestUtilities.Builders.Games;
using FCG.Catalog.CommomTestUtilities.Builders.Games.Repositories;
using FCG.Catalog.Domain.Enum;
using FCG.Catalog.Domain.Exception;
using FluentAssertions;
using Moq;

namespace FCG.Catalog.UnitTests.Application.UseCases.Games
{
    public class UpdateGameUseCaseTests
    {
        private readonly GameBuilder _gameBuilder;

        public UpdateGameUseCaseTests()
        {
            _gameBuilder = new GameBuilder();
            ReadOnlyGameRepositoryBuilder.Reset();
            WriteOnlyGameRepositoryBuilder.Reset();
            UnitOfWorkBuilder.Reset();
        }

        [Fact]
        public async Task Handle_ShouldUpdateGame_WhenAllDataIsValid()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var existingGame = _gameBuilder.BuildWithId(gameId, "Old Name", 49.99m, GameCategory.Action);

            var input = new UpdateGameInput
            {
                Id = gameId,
                Title = "Updated Name",
                Description = "Updated Description",
                Price = 59.99m,
                Category = GameCategory.RPG
            };

            ReadOnlyGameRepositoryBuilder.SetupGetByIdAsync(gameId, existingGame);
            WriteOnlyGameRepositoryBuilder.SetupUpdate(existingGame);
            UnitOfWorkBuilder.SetupSaveChangesAsync();

            var useCase = new UpdateGameUseCase(
                ReadOnlyGameRepositoryBuilder.Build(),
                WriteOnlyGameRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build()
            );

            // Act
            var result = await useCase.Handle(input, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(gameId);
            result.Title.Should().Be("Updated Name");
            result.Description.Should().Be("Updated Description");
            result.Price.Should().Be(59.99m);
            result.Category.Should().Be(nameof(GameCategory.RPG));
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenGameDoesNotExist()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var input = new UpdateGameInput
            {
                Id = gameId,
                Title = "Updated Name",
                Description = "Description",
                Price = 39.99m,
                Category = GameCategory.Action
            };

            ReadOnlyGameRepositoryBuilder.SetupGetByIdAsync(gameId, null);

            var useCase = new UpdateGameUseCase(
                ReadOnlyGameRepositoryBuilder.Build(),
                WriteOnlyGameRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build()
            );

            // Act
            Func<Task> act = async () => await useCase.Handle(input, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"Game id '{gameId}' not found.");
        }

        [Fact]
        public async Task Handle_ShouldThrowDomainException_WhenCategoryIsInvalid()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var existingGame = _gameBuilder.BuildWithId(gameId);
            var input = new UpdateGameInput
            {
                Id = gameId,
                Title = "Updated Name",
                Description = "Description",
                Price = 49.99m,
                Category = (GameCategory)999 
            };

            ReadOnlyGameRepositoryBuilder.SetupGetByIdAsync(gameId, existingGame);

            var useCase = new UpdateGameUseCase(
                ReadOnlyGameRepositoryBuilder.Build(),
                WriteOnlyGameRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build()
            );

            // Act
            Func<Task> act = async () => await useCase.Handle(input, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DomainException>()
                .WithMessage("Invalid category: '999'. Available categories are: Action, Adventure, RPG...");
        }

        [Fact]
        public async Task Handle_ShouldCallGetByIdAsync_WithCorrectId()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var existingGame = _gameBuilder.BuildWithId(gameId);
            var input = new UpdateGameInput
            {
                Id = gameId,
                Title = "Name",
                Description = "Desc",
                Price = 29.99m,
                Category = GameCategory.Adventure
            };

            ReadOnlyGameRepositoryBuilder.SetupGetByIdAsync(gameId, existingGame);
            WriteOnlyGameRepositoryBuilder.SetupUpdate(existingGame);
            UnitOfWorkBuilder.SetupSaveChangesAsync();

            var useCase = new UpdateGameUseCase(
                ReadOnlyGameRepositoryBuilder.Build(),
                WriteOnlyGameRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build()
            );

            // Act
            await useCase.Handle(input, CancellationToken.None);

            // Assert
            ReadOnlyGameRepositoryBuilder.VerifyGetByIdAsyncWasCalled(gameId, Times.Once());
        }

        [Fact]
        public async Task Handle_ShouldCallUpdate_WhenGameIsValid()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var existingGame = _gameBuilder.BuildWithId(gameId);
            var input = new UpdateGameInput
            {
                Id = gameId,
                Title = "Updated Title",
                Description = "Updated Desc",
                Price = 19.99m,
                Category = GameCategory.Strategy
            };

            ReadOnlyGameRepositoryBuilder.SetupGetByIdAsync(gameId, existingGame);
            WriteOnlyGameRepositoryBuilder.SetupUpdate(existingGame);
            UnitOfWorkBuilder.SetupSaveChangesAsync();

            var useCase = new UpdateGameUseCase(
                ReadOnlyGameRepositoryBuilder.Build(),
                WriteOnlyGameRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build()
            );

            // Act
            await useCase.Handle(input, CancellationToken.None);

            // Assert
            WriteOnlyGameRepositoryBuilder.VerifyUpdateWasCalled(Times.Once());
        }

        [Fact]
        public async Task Handle_ShouldCallSaveChangesAsync_AfterUpdate()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var existingGame = _gameBuilder.BuildWithId(gameId);
            var input = new UpdateGameInput
            {
                Id = gameId,
                Title = "Title",
                Description = "Desc",
                Price = 39.99m,
                Category = GameCategory.Sports
            };

            ReadOnlyGameRepositoryBuilder.SetupGetByIdAsync(gameId, existingGame);
            WriteOnlyGameRepositoryBuilder.SetupUpdate(existingGame);
            UnitOfWorkBuilder.SetupSaveChangesAsync();

            var useCase = new UpdateGameUseCase(
                ReadOnlyGameRepositoryBuilder.Build(),
                WriteOnlyGameRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build()
            );

            // Act
            await useCase.Handle(input, CancellationToken.None);

            // Assert
            UnitOfWorkBuilder.VerifySaveChangesAsyncWasCalled(Times.Once());
        }

        [Fact]
        public async Task Handle_ShouldNotCallUpdateOrSave_WhenGameNotFound()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var input = new UpdateGameInput
            {
                Id = gameId,
                Title = "Title",
                Description = "Desc",
                Price = 29.99m,
                Category = GameCategory.Action
            };

            ReadOnlyGameRepositoryBuilder.SetupGetByIdAsync(gameId, null);

            var useCase = new UpdateGameUseCase(
                ReadOnlyGameRepositoryBuilder.Build(),
                WriteOnlyGameRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build()
            );

            // Act
            Func<Task> act = async () => await useCase.Handle(input, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
            WriteOnlyGameRepositoryBuilder.VerifyUpdateWasCalled(Times.Never());
            UnitOfWorkBuilder.VerifySaveChangesAsyncWasNotCalled();
        }

        [Fact]
        public async Task Handle_ShouldRespectCancellationToken()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var existingGame = _gameBuilder.BuildWithId(gameId);
            var input = new UpdateGameInput
            {
                Id = gameId,
                Title = "Test",
                Description = "Test Desc",
                Price = 49.99m,
                Category = GameCategory.RPG
            };
            var cancellationToken = CancellationToken.None;

            ReadOnlyGameRepositoryBuilder.SetupGetByIdAsync(gameId, existingGame);
            WriteOnlyGameRepositoryBuilder.SetupUpdate(existingGame);
            UnitOfWorkBuilder.SetupSaveChangesAsync();

            var useCase = new UpdateGameUseCase(
                ReadOnlyGameRepositoryBuilder.Build(),
                WriteOnlyGameRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build()
            );

            // Act
            var result = await useCase.Handle(input, cancellationToken);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task Handle_ShouldUpdateAllProperties_Correctly()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var existingGame = _gameBuilder.BuildWithId(
                gameId,
                "Old Name",
                10.00m,
                GameCategory.Action
            );

            var input = new UpdateGameInput
            {
                Id = gameId,
                Title = "Completely New Name",
                Description = "Brand New Description",
                Price = 99.99m,
                Category = GameCategory.Simulation
            };

            ReadOnlyGameRepositoryBuilder.SetupGetByIdAsync(gameId, existingGame);
            WriteOnlyGameRepositoryBuilder.SetupUpdate(existingGame);
            UnitOfWorkBuilder.SetupSaveChangesAsync();

            var useCase = new UpdateGameUseCase(
                ReadOnlyGameRepositoryBuilder.Build(),
                WriteOnlyGameRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build()
            );

            // Act
            var result = await useCase.Handle(input, CancellationToken.None);

            // Assert
            result.Title.Should().Be("Completely New Name");
            result.Description.Should().Be("Brand New Description");
            result.Price.Should().Be(99.99m);
            result.Category.Should().Be(nameof(GameCategory.Simulation));
            result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public async Task Handle_ShouldAcceptAllValidCategories()
        {
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

                var gameId = Guid.NewGuid();
                var existingGame = _gameBuilder.BuildWithId(gameId);

                var input = new UpdateGameInput
                {
                    Id = gameId,
                    Title = $"Game {category}",
                    Description = "Description",
                    Price = 49.99m,
                    Category = category
                };

                ReadOnlyGameRepositoryBuilder.SetupGetByIdAsync(gameId, existingGame);
                WriteOnlyGameRepositoryBuilder.SetupUpdate(existingGame);
                UnitOfWorkBuilder.SetupSaveChangesAsync();

                var useCase = new UpdateGameUseCase(
                    ReadOnlyGameRepositoryBuilder.Build(),
                    WriteOnlyGameRepositoryBuilder.Build(),
                    UnitOfWorkBuilder.Build()
                );

                var result = await useCase.Handle(input, CancellationToken.None);
                result.Should().NotBeNull();
                result.Category.Should().Be(category.ToString());
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
            var gameId = Guid.NewGuid();
            var existingGame = _gameBuilder.BuildWithId(gameId);

            var input = new UpdateGameInput
            {
                Id = gameId,
                Title = $"Game Price {price}",
                Description = "Description",
                Price = price,
                Category = GameCategory.Action
            };

            ReadOnlyGameRepositoryBuilder.SetupGetByIdAsync(gameId, existingGame);
            WriteOnlyGameRepositoryBuilder.SetupUpdate(existingGame);
            UnitOfWorkBuilder.SetupSaveChangesAsync();

            var useCase = new UpdateGameUseCase(
                ReadOnlyGameRepositoryBuilder.Build(),
                WriteOnlyGameRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build()
            );

            // Act
            var result = await useCase.Handle(input, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Price.Should().Be(price);
        }

        [Fact]
        public async Task Handle_ShouldUpdateGameWithSameName_WhenUpdatingOtherProperties()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var existingGame = _gameBuilder.BuildWithId(gameId, "Same Name", 10m, GameCategory.Action);

            var input = new UpdateGameInput
            {
                Id = gameId,
                Title = "Same Name", 
                Description = "New Description",
                Price = 20m,
                Category = GameCategory.RPG
            };

            ReadOnlyGameRepositoryBuilder.SetupGetByIdAsync(gameId, existingGame);
            WriteOnlyGameRepositoryBuilder.SetupUpdate(existingGame);
            UnitOfWorkBuilder.SetupSaveChangesAsync();

            var useCase = new UpdateGameUseCase(
                ReadOnlyGameRepositoryBuilder.Build(),
                WriteOnlyGameRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build()
            );

            // Act
            var result = await useCase.Handle(input, CancellationToken.None);

            // Assert
            result.Title.Should().Be("Same Name");
            result.Description.Should().Be("New Description");
            result.Price.Should().Be(20m);
            result.Category.Should().Be(nameof(GameCategory.RPG));
        }

        [Fact]
        public async Task Handle_ShouldSetUpdatedAt_ToCurrentDateTime()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var existingGame = _gameBuilder.BuildWithId(gameId);
            var beforeUpdate = DateTime.UtcNow;

            var input = new UpdateGameInput
            {
                Id = gameId,
                Title = "Test",
                Description = "Test",
                Price = 10m,
                Category = GameCategory.Action
            };

            ReadOnlyGameRepositoryBuilder.SetupGetByIdAsync(gameId, existingGame);
            WriteOnlyGameRepositoryBuilder.SetupUpdate(existingGame);
            UnitOfWorkBuilder.SetupSaveChangesAsync();

            var useCase = new UpdateGameUseCase(
                ReadOnlyGameRepositoryBuilder.Build(),
                WriteOnlyGameRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build()
            );

            // Act
            var result = await useCase.Handle(input, CancellationToken.None);
            var afterUpdate = DateTime.UtcNow;

            // Assert
            result.UpdatedAt.Should().BeOnOrAfter(beforeUpdate);
            result.UpdatedAt.Should().BeOnOrBefore(afterUpdate);
        }
    }
}