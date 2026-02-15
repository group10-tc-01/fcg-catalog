using FCG.Catalog.Domain.Catalog.Entities.Games;
using FCG.Catalog.Domain.Catalog.ValueObjects;
using FCG.Catalog.Domain.Enum;
using FCG.Catalog.Domain.Exception;
using FCG.Catalog.Messages;
using FluentAssertions;

namespace FCG.Catalog.UnitTests.Domain.Entities
{
    public class GameTests
    {
        [Fact]
        public void Given_ValidData_When_Create_Then_ShouldCreateGameSuccessfully()
        {
            // Arrange
            var title = Title.Create("FIFA 2023");
            var description = "A great soccer game";
            var price = Price.Create(59.99m);
            var category = GameCategory.Sports;

            // Act
            var game = Game.Create(title.Value, description, price, category);

            // Assert
            game.Should().NotBeNull();
            game.Title.Value.Should().Be(title.Value);
            game.Description.Should().Be(description);
            game.Price.Value.Should().Be(price.Value);
            game.Category.Should().Be(category);
        }

        [Fact]
        public void Given_InvalidTitle_When_Create_Then_ShouldThrowDomainException()
        {
            // Arrange
            var invalidTitle = string.Empty;
            var description = "Description";
            var price = Price.Create(10m);
            var category = GameCategory.Action;

            // Act
            var act = () => Game.Create(invalidTitle, description, price, category);

            // Assert
            act.Should().Throw<DomainException>().WithMessage(ResourceMessages.GameNameIsRequired);
        }

        // Adicione mais testes para Update, GetActivePromotion, etc.
    }
}