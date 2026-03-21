using FCG.Catalog.CommomTestUtilities.Builders.Games;
using FCG.Catalog.Domain.Catalog.Entities.Games;
using FCG.Catalog.Domain.Catalog.Entities.Promotions;
using FCG.Catalog.Domain.Catalog.ValueObjects;
using FCG.Catalog.Domain.Enum;
using FCG.Catalog.Domain.Exception;
using FCG.Catalog.Messages;
using FluentAssertions;

namespace FCG.Catalog.UnitTests.Domain.Catalog
{
    public class GameTests
    {
        [Fact]
        public void Given_ValidGameParameters_When_Create_Then_ShouldInstantiateGameCorrectly()
        {
            // Arrange
            var gameEntity = new GameBuilder().Build();

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
            var gameEntity = new GameBuilder().Build();
            var actShortName = () => Game.Create(Title.Create("A"), gameEntity.Description, Price.Create(gameEntity.Price), gameEntity.Category);
            var actNullName = () => Game.Create(Title.Create(""), gameEntity.Description, Price.Create(gameEntity.Price), gameEntity.Category);

            // Act & Assert
            actShortName.Should().Throw<DomainException>()
                .WithMessage(ResourceMessages.GameTitleMinLength);

            actNullName.Should().Throw<DomainException>().WithMessage(ResourceMessages.GameNameIsRequired);
        }

        [Fact]
        public void Given_ValidParameters_When_Update_Then_ShouldUpdateGameSuccessfully()
        {
            // Arrange
            var game = new GameBuilder().Build();
            var newTitle = Title.Create("Updated Title");
            var newDescription = "Updated Description";
            var newPrice = Price.Create(79.99m);
            var newCategory = GameCategory.Action;
            var updatedAt = DateTime.UtcNow;

            // Act
            game.Update(newTitle, newDescription, newPrice, newCategory, updatedAt);

            // Assert
            game.Title.Value.Should().Be(newTitle.Value);
            game.Description.Should().Be(newDescription);
            game.Price.Value.Should().Be(newPrice.Value);
            game.Category.Should().Be(newCategory);
            game.UpdatedAt.Should().Be(updatedAt);
        }

        [Fact]
        public void Given_InvalidParameters_When_Update_Then_ShouldThrowDomainException()
        {
            // Arrange
            var game = new GameBuilder().Build();
            var invalidTitle = "";  // Passa uma string vazia diretamente
            var newDescription = "Updated Description";
            var newPrice = Price.Create(79.99m);
            var newCategory = GameCategory.Action;
            var updatedAt = DateTime.UtcNow;

            // Act
            var act = () => game.Update(invalidTitle, newDescription, newPrice, newCategory, updatedAt);

            // Assert
            act.Should().Throw<DomainException>().WithMessage(ResourceMessages.GameNameIsRequired);
        }

        [Fact]
        public void Given_NoPromotions_When_GetActivePromotion_Then_ShouldReturnNull()
        {
            // Arrange
            var game = new GameBuilder().Build();

            // Act
            var activePromotion = game.GetActivePromotion();

            // Assert
            activePromotion.Should().BeNull();
        }

        [Fact]
        public void Given_ActivePromotion_When_GetActivePromotion_Then_ShouldReturnPromotion()
        {
            // Arrange
            var game = new GameBuilder().BuildWithPromotion(100m, 20m);

            // Act
            var activePromotion = game.GetActivePromotion();

            // Assert
            activePromotion.Should().NotBeNull();
            activePromotion!.DiscountPercentage.Value.Should().Be(20m);
        }

        [Fact]
        public void Given_NoActivePromotion_When_CalculateDiscountedPrice_Then_ShouldReturnOriginalPrice()
        {
            // Arrange
            var game = new GameBuilder().BuildWithPrice(100m);

            // Act
            var discountedPrice = game.CalculateDiscountedPrice(null);

            // Assert
            discountedPrice.Should().Be(100m);
        }

        [Fact]
        public void Given_ActivePromotion_When_CalculateDiscountedPrice_Then_ShouldReturnDiscountedPrice()
        {
            // Arrange
            var game = new GameBuilder().BuildWithPrice(100m);
            var promotion = Promotion.Create(game.Id, Discount.Create(20m), DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));

            // Act
            var discountedPrice = game.CalculateDiscountedPrice(promotion);

            // Assert
            discountedPrice.Should().Be(80m);
        }
    }
}
