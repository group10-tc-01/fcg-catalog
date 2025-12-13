using FCG.Catalog.Domain.Catalog.Entity.Libraries;
using FCG.CommomTestsUtilities.Builders.Entities;
using FluentAssertions;

namespace FCG.Catalog.Domain.Catalog
{
    public class LibraryTests
    {
        [Fact]
        public void Given_ValidUserId_When_CreateLibrary_Then_ShouldCreateSuccessfully()
        {
            var userId = Guid.NewGuid();

            var library = Library.Create(userId);

            library.Should().NotBeNull();
            library.Id.Should().NotBe(Guid.Empty);
            library.UserId.Should().Be(userId);
        }

        [Fact]
        public void Given_EmptyUserId_When_CreateLibrary_Then_ShouldCreateWithEmptyId()
        {
            var emptyUserId = Guid.Empty;

            var library = Library.Create(emptyUserId);

            library.Should().NotBeNull();
            library.UserId.Should().Be(Guid.Empty);
        }

        [Fact]
        public void Given_ValidUserId_When_CreateLibraryUsingConstructor_Then_ShouldCreateSuccessfully()
        {
            var userId = Guid.NewGuid();

            var library = new Library(userId);

            library.Should().NotBeNull();
            library.UserId.Should().Be(userId);
        }

        [Fact]
        public void Build_WhenCalled_ShouldReturnValidLibraryInstance()
        {
            var library = LibraryBuilder.Build();

            library.Should().NotBeNull();
            library.UserId.Should().NotBe(Guid.Empty);
            library.LibraryGames.Should().NotBeNull("A coleção deve ser inicializada no construtor.");
            library.LibraryGames.Should().BeEmpty();
        }
    }
}