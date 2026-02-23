using FCG.Catalog.Domain.Catalog.Entities.Promotions;
using FCG.Catalog.Domain.Catalog.ValueObjects;
using FCG.Catalog.Domain.Exception;
using FCG.Catalog.Messages;
using FluentAssertions;

namespace FCG.Catalog.UnitTests.Domain.Entities
{
    public class PromotionTests
    {
        [Fact]
        public void Given_ValidData_When_Create_Then_ShouldCreatePromotionSuccessfully()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var discount = Discount.Create(15m);
            var startDate = DateTime.UtcNow.AddDays(1);
            var endDate = DateTime.UtcNow.AddDays(10);

            // Act
            var promotion = Promotion.Create(gameId, discount, startDate, endDate);

            // Assert
            promotion.Should().NotBeNull();
            promotion.GameId.Should().Be(gameId);
            promotion.DiscountPercentage.Value.Should().Be(discount.Value);
            promotion.StartDate.Should().Be(startDate);
            promotion.EndDate.Should().Be(endDate);
        }

        [Fact]
        public void Given_EndDateBeforeStartDate_When_Create_Then_ShouldThrowDomainException()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var discount = Discount.Create(10m);
            var startDate = DateTime.UtcNow.AddDays(10);
            var endDate = DateTime.UtcNow.AddDays(1);

            // Act
            var act = () => Promotion.Create(gameId, discount, startDate, endDate);

            // Assert
            act.Should().Throw<DomainException>().WithMessage(ResourceMessages.PromotionEndDateMustBeAfterStartDate);
        }

        [Fact]
        public void Given_ValidData_When_Update_Then_ShouldUpdatePromotionSuccessfully()
        {
            // Arrange
            var promotion = Promotion.Create(Guid.NewGuid(), Discount.Create(10m), DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(5));
            var newGameId = Guid.NewGuid();
            var newDiscount = Discount.Create(20m);
            var newStartDate = DateTime.UtcNow.AddDays(2);
            var newEndDate = DateTime.UtcNow.AddDays(7);

            // Act
            promotion.Update(newGameId, newDiscount, newStartDate, newEndDate);

            // Assert
            promotion.GameId.Should().Be(newGameId);
            promotion.DiscountPercentage.Value.Should().Be(newDiscount.Value);
            promotion.StartDate.Should().Be(newStartDate);
            promotion.EndDate.Should().Be(newEndDate);
        }

        [Fact]
        public void Given_InvalidDates_When_Update_Then_ShouldThrowDomainException()
        {
            // Arrange
            var promotion = Promotion.Create(Guid.NewGuid(), Discount.Create(10m), DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(5));
            var newGameId = Guid.NewGuid();
            var newDiscount = Discount.Create(15m);
            var invalidStartDate = DateTime.UtcNow.AddDays(10);
            var invalidEndDate = DateTime.UtcNow.AddDays(5);

            // Act
            var act = () => promotion.Update(newGameId, newDiscount, invalidStartDate, invalidEndDate);

            // Assert
            act.Should().Throw<DomainException>().WithMessage(ResourceMessages.PromotionEndDateMustBeAfterStartDate);
        }
    }
}