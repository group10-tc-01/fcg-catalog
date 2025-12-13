using FCG.Catalog.Domain.Catalog.Entity.Games;
using FCG.Catalog.Domain.Catalog.ValueObjects;
using FCG.Catalog.Domain.Exception;
using FCG.Catalog.Messages;
using FCG.CommomTestsUtilities.Builders.Entities;
using FluentAssertions;

namespace FCG.Catalog.Domain.Catalog
{
    public class GameTests
    {
        [Fact]
        public void Given_ValidGameParameters_When_Create_Then_ShouldInstantiateGameCorrectly()
        {
            // Arrange
            var gameEntity = GameBuilder.Build();

            // Act
            var game = Game.Create(Title.Create(gameEntity.Title), gameEntity.Description, Price.Create(gameEntity.Price), gameEntity.Category);

            // Assert
            game.Should().NotBeNull();
            game.Id.Should().NotBe(Guid.Empty);
            game.Title.Value.Should().Be(gameEntity.Title);
            game.Description.Should().Be(gameEntity.Description);
            game.Price.Value.Should().Be(gameEntity.Price);
            game.Category.Should().Be(gameEntity.Category);
        }

        [Fact]
        public void Given_InvalidName_When_Create_Then_ShouldThrowDomainException()
        {
            // Arrange
            var gameEntity = GameBuilder.Build();
            var actShortName = () => Game.Create(Title.Create("A"), gameEntity.Description, Price.Create(gameEntity.Price), gameEntity.Category);
            var actNullName = () => Game.Create(Title.Create(""), gameEntity.Description, Price.Create(gameEntity.Price), gameEntity.Category);

            // Act & Assert
            actShortName.Should().Throw<DomainException>()
                .WithMessage(ResourceMessages.GameTitleMinLength);

            actNullName.Should().Throw<DomainException>().WithMessage(ResourceMessages.GameNameIsRequired);
        }
    }
}
