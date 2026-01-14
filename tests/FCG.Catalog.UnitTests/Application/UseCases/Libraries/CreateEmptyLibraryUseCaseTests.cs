using FCG.Catalog.Application.UseCases.Libraries.Register;
using FCG.Catalog.CommomTestUtilities.Builders;
using FCG.Catalog.CommomTestUtilities.Builders.Libraries;
using FCG.Catalog.CommomTestUtilities.Builders.Libraries.Repositories;
using FCG.Catalog.Domain.Abstractions;
using FCG.Catalog.Domain.Catalog.Entities.Libraries;
using FCG.Catalog.Domain.Exception;
using FCG.Domain.Repositories.LibraryRepository;
using FluentAssertions;
using Moq;

namespace FCG.Catalog.UnitTests.Application.UseCases.Libraries
{
    public class CreateEmptyLibraryUseCaseTests
    {
        private readonly LibraryBuilder _libraryBuilder;

        public CreateEmptyLibraryUseCaseTests()
        {
            _libraryBuilder = new LibraryBuilder();
            ReadOnlyLibraryRepositoryBuilder.Reset();
            WriteOnlyLibraryRepositoryBuilder.Reset();
            UnitOfWorkBuilder.Reset();
        }

        [Fact]
        public async Task Handle_ShouldCreateEmptyLibrary_WhenUserDoesNotHaveLibrary()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new CreateEmptyLibraryCommand(userId);

            ReadOnlyLibraryRepositoryBuilder.SetupGetByUserIdAsync(userId, null);
            WriteOnlyLibraryRepositoryBuilder.SetupAddAsync();
            UnitOfWorkBuilder.SetupSaveChangesAsync();

            var useCase = new CreateEmptyLibraryUseCase(
                ReadOnlyLibraryRepositoryBuilder.Build(),
                WriteOnlyLibraryRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build()
            );

            // Act
            await useCase.Handle(command, CancellationToken.None);

            // Assert
            ReadOnlyLibraryRepositoryBuilder.VerifyGetByUserIdAsyncWasCalled(userId, Times.Once());
            WriteOnlyLibraryRepositoryBuilder.VerifyAddAsyncWasCalledWithLibrary(userId, Times.Once());
            UnitOfWorkBuilder.VerifySaveChangesAsyncWasCalled(Times.Once());
        }

        [Fact]
        public async Task Handle_ShouldThrowConflictException_WhenLibraryAlreadyExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new CreateEmptyLibraryCommand(userId);

            var existingLibrary = _libraryBuilder.BuildWithUserId(userId);

            ReadOnlyLibraryRepositoryBuilder.SetupGetByUserIdAsync(userId, existingLibrary);

            var useCase = new CreateEmptyLibraryUseCase(
                ReadOnlyLibraryRepositoryBuilder.Build(),
                WriteOnlyLibraryRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build()
            );

            // Act
            Func<Task> act = async () => await useCase.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ConflictException>()
                .WithMessage($"*{userId}*");

            ReadOnlyLibraryRepositoryBuilder.VerifyGetByUserIdAsyncWasCalled(userId, Times.Once());
            WriteOnlyLibraryRepositoryBuilder.VerifyAddAsyncWasCalled(Times.Never());
            UnitOfWorkBuilder.VerifySaveChangesAsyncWasCalled(Times.Never());
        }

        [Fact]
        public async Task Handle_ShouldCallRepositoryInCorrectOrder_WhenCreatingLibrary()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new CreateEmptyLibraryCommand(userId);
            var callOrder = new List<string>();

            ReadOnlyLibraryRepositoryBuilder.SetupGetByUserIdAsync(userId, null);

            WriteOnlyLibraryRepositoryBuilder.Reset();
            var writeOnlyMock = new Mock<IWriteOnlyLibraryRepository>();
            writeOnlyMock.Setup(repo => repo.AddAsync(It.IsAny<Library>()))
                .Callback(() => callOrder.Add("AddAsync"))
                .Returns(Task.CompletedTask);

            UnitOfWorkBuilder.Reset();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Callback(() => callOrder.Add("SaveChanges"))
                .ReturnsAsync(1);

            var useCase = new CreateEmptyLibraryUseCase(
                ReadOnlyLibraryRepositoryBuilder.Build(),
                writeOnlyMock.Object,
                unitOfWorkMock.Object
            );

            // Act
            await useCase.Handle(command, CancellationToken.None);

            // Assert
            callOrder.Should().HaveCount(2);
            callOrder[0].Should().Be("AddAsync");
            callOrder[1].Should().Be("SaveChanges");
        }

        [Fact]
        public async Task Handle_ShouldCreateLibraryWithCorrectUserId_WhenCalled()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new CreateEmptyLibraryCommand(userId);

            ReadOnlyLibraryRepositoryBuilder.SetupGetByUserIdAsync(userId, null);
            WriteOnlyLibraryRepositoryBuilder.SetupAddAsync();
            UnitOfWorkBuilder.SetupSaveChangesAsync();

            var useCase = new CreateEmptyLibraryUseCase(
                ReadOnlyLibraryRepositoryBuilder.Build(),
                WriteOnlyLibraryRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build()
            );

            // Act
            await useCase.Handle(command, CancellationToken.None);

            // Assert
            WriteOnlyLibraryRepositoryBuilder.VerifyAddAsyncWasCalledWithLibrary(userId, Times.Once());
        }

        [Fact]
        public async Task Handle_ShouldNotSaveChanges_WhenLibraryAlreadyExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new CreateEmptyLibraryCommand(userId);

            var existingLibrary = _libraryBuilder.BuildWithUserId(userId);
            ReadOnlyLibraryRepositoryBuilder.SetupGetByUserIdAsync(userId, existingLibrary);

            var useCase = new CreateEmptyLibraryUseCase(
                ReadOnlyLibraryRepositoryBuilder.Build(),
                WriteOnlyLibraryRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build()
            );

            // Act
            try
            {
                await useCase.Handle(command, CancellationToken.None);
            }
            catch (ConflictException)
            {
                // Esperado
            }

            // Assert
            UnitOfWorkBuilder.VerifySaveChangesAsyncWasCalled(Times.Never());
        }

        [Fact]
        public async Task Handle_ShouldCheckExistingLibraryBeforeCreating_WhenCalled()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new CreateEmptyLibraryCommand(userId);

            ReadOnlyLibraryRepositoryBuilder.SetupGetByUserIdAsync(userId, null);
            WriteOnlyLibraryRepositoryBuilder.SetupAddAsync();
            UnitOfWorkBuilder.SetupSaveChangesAsync();

            var useCase = new CreateEmptyLibraryUseCase(
                ReadOnlyLibraryRepositoryBuilder.Build(),
                WriteOnlyLibraryRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build()
            );

            // Act
            await useCase.Handle(command, CancellationToken.None);

            // Assert
            ReadOnlyLibraryRepositoryBuilder.VerifyGetByUserIdAsyncWasCalled(userId, Times.Once());
        }

        [Theory]
        [InlineData("00000000-0000-0000-0000-000000000000")]
        [InlineData("550e8400-e29b-41d4-a716-446655440000")]
        [InlineData("f47ac10b-58cc-4372-a567-0e02b2c3d479")]
        public async Task Handle_ShouldCreateLibraryForAnyValidUserId_WhenCalled(string userIdString)
        {
            // Arrange
            var userId = Guid.Parse(userIdString);
            var command = new CreateEmptyLibraryCommand(userId);

            ReadOnlyLibraryRepositoryBuilder.SetupGetByUserIdAsync(userId, null);
            WriteOnlyLibraryRepositoryBuilder.SetupAddAsync();
            UnitOfWorkBuilder.SetupSaveChangesAsync();

            var useCase = new CreateEmptyLibraryUseCase(
                ReadOnlyLibraryRepositoryBuilder.Build(),
                WriteOnlyLibraryRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build()
            );

            // Act
            await useCase.Handle(command, CancellationToken.None);

            // Assert
            WriteOnlyLibraryRepositoryBuilder.VerifyAddAsyncWasCalledWithLibrary(userId, Times.Once());
            UnitOfWorkBuilder.VerifySaveChangesAsyncWasCalled(Times.Once());
        }

        [Fact]
        public async Task Handle_ShouldPersistChanges_WhenLibraryIsCreatedSuccessfully()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new CreateEmptyLibraryCommand(userId);

            ReadOnlyLibraryRepositoryBuilder.SetupGetByUserIdAsync(userId, null);
            WriteOnlyLibraryRepositoryBuilder.SetupAddAsync();
            UnitOfWorkBuilder.SetupSaveChangesAsync();

            var useCase = new CreateEmptyLibraryUseCase(
                ReadOnlyLibraryRepositoryBuilder.Build(),
                WriteOnlyLibraryRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build()
            );

            // Act
            await useCase.Handle(command, CancellationToken.None);

            // Assert
            UnitOfWorkBuilder.VerifySaveChangesAsyncWasCalled(Times.Once());
        }

        [Fact]
        public async Task Handle_ShouldThrowConflictExceptionWithCorrectMessage_WhenLibraryExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new CreateEmptyLibraryCommand(userId);

            var existingLibrary = _libraryBuilder.BuildWithUserId(userId);
            ReadOnlyLibraryRepositoryBuilder.SetupGetByUserIdAsync(userId, existingLibrary);

            var useCase = new CreateEmptyLibraryUseCase(
                ReadOnlyLibraryRepositoryBuilder.Build(),
                WriteOnlyLibraryRepositoryBuilder.Build(),
                UnitOfWorkBuilder.Build()
            );

            // Act
            Func<Task> act = async () => await useCase.Handle(command, CancellationToken.None);

            // Assert
            var exception = await act.Should().ThrowAsync<ConflictException>();
            exception.Which.Message.Should().Contain(userId.ToString());
        }
    }
}