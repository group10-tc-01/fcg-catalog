using FCG.Catalog.Domain.Catalog.Entity.Promotions;
using FCG.Catalog.Domain.Exception;
using FCG.Catalog.Messages;
using FCG.CommomTestsUtilities.Builders.Entities;
using FluentAssertions;

namespace FCG.UnitTests.Domain.Entities
{
    public class PromotionTests
    {
        [Fact]
        public void Given_ValidParameters_When_Create_Then_ShouldSetAllPropertiesCorrectly()
        {
            var game = GameBuilder.Build();
            var discountValue = 25.5m;
            var startDate = DateTime.UtcNow.Date;
            var endDate = DateTime.UtcNow.Date.AddDays(7);

            var promotion = Promotion.Create(game.Id, discountValue, startDate, endDate);

            promotion.Should().NotBeNull();
            promotion.Id.Should().NotBe(Guid.Empty);
            promotion.GameId.Should().Be(game.Id);
            promotion.DiscountPercentage.Value.Should().Be(discountValue);
            promotion.StartDate.Should().Be(startDate);
            promotion.EndDate.Should().Be(endDate);
        }

        [Fact]
        public void Given_ZeroAndMaxDiscount_When_Create_Then_ShouldHandleBothValues()
        {
            var game = GameBuilder.Build();
            var startDate = DateTime.UtcNow.Date;
            var endDate = DateTime.UtcNow.Date.AddDays(3);
            var zeroDiscount = 0m;
            var maxDiscount = 100m;

            var zeroPromotion = Promotion.Create(game.Id, zeroDiscount, startDate, endDate);
            var maxPromotion = Promotion.Create(game.Id, maxDiscount, startDate, endDate);

            zeroPromotion.DiscountPercentage.Value.Should().Be(0m);
            maxPromotion.DiscountPercentage.Value.Should().Be(100m);
        }

        [Fact]
        public void Given_EndDateBeforeStartDate_When_Create_Then_ShouldThrowDomainException()
        {
            var game = GameBuilder.Build();
            var discount = 20m;
            var startDate = DateTime.UtcNow;
            var endDate = DateTime.UtcNow.AddDays(-1);
            var act = () => Promotion.Create(game.Id, discount, startDate, endDate);

            act.Should().Throw<DomainException>()
               .WithMessage(ResourceMessages.PromotionEndDateMustBeAfterStartDate);
        }

        [Fact]
        public void Given_DiscountOutOfRange_When_Create_Then_ShouldThrowDomainException()
        {
            var game = GameBuilder.Build();
            var startDate = DateTime.UtcNow;
            var endDate = DateTime.UtcNow.AddDays(2);
            var negativeDiscount = -5m;
            var invalidDiscount = 101m;
            var actNegative = () => Promotion.Create(game.Id, negativeDiscount, startDate, endDate);
            var actAbove100 = () => Promotion.Create(game.Id, invalidDiscount, startDate, endDate);

            actNegative.Should().Throw<DomainException>()
                       .WithMessage(ResourceMessages.DiscountMustBeBetweenZeroAndHundred);
            actAbove100.Should().Throw<DomainException>()
                       .WithMessage(ResourceMessages.DiscountMustBeBetweenZeroAndHundred);
        }

        [Fact]
        public void Given_SameStartAndEndDate_When_Create_Then_ShouldCreateSuccessfully()
        {
            var game = GameBuilder.Build();
            var discount = 30m;
            var sameDate = DateTime.UtcNow.Date;

            var promotion = Promotion.Create(game.Id, discount, sameDate, sameDate);

            promotion.StartDate.Should().Be(sameDate);
            promotion.EndDate.Should().Be(sameDate);
        }

        [Fact]
        public void Given_TwoPromotionsWithSameData_When_Create_Then_ShouldHaveDifferentIds()
        {
            var game = GameBuilder.Build();
            var discount = 25m;
            var startDate = DateTime.UtcNow.Date;
            var endDate = DateTime.UtcNow.Date.AddDays(7);

            var promotion1 = Promotion.Create(game.Id, discount, startDate, endDate);
            var promotion2 = Promotion.Create(game.Id, discount, startDate, endDate);

            promotion1.Id.Should().NotBe(promotion2.Id);
            promotion1.GameId.Should().Be(promotion2.GameId);
            promotion1.DiscountPercentage.Should().Be(promotion2.DiscountPercentage);
            promotion1.StartDate.Should().Be(promotion2.StartDate);
            promotion1.EndDate.Should().Be(promotion2.EndDate);
        }

    }
}