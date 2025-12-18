using FCG.Catalog.Domain.Catalog.ValueObjects;
using FCG.Catalog.Domain.Exception;
using FCG.Catalog.Messages;
using FluentAssertions;
using System.Globalization;

namespace FCG.Catalog.UnitTests.Domain.ValueObjects
{
    public class DiscountTests
    {
        [Fact]
        public void Given_ValidPercentages_When_Create_Then_ShouldCreateSuccessfullyAndSetProperties()
        {
            decimal validPercentage = 25.5m;
            decimal zeroPercentage = 0m;
            decimal maxPercentage = 100m;

            var discount = Discount.Create(validPercentage);
            var zeroDiscount = Discount.Create(zeroPercentage);
            var maxDiscount = Discount.Create(maxPercentage);

            discount.Should().NotBeNull();
            discount.Value.Should().Be(validPercentage);
            zeroDiscount.Value.Should().Be(0);
            maxDiscount.Value.Should().Be(100);
        }

        [Fact]
        public void Given_InvalidPercentages_When_Create_Then_ShouldThrowDomainException()
        {
            decimal negativePercentage = -5m;
            decimal invalidPercentage = 101m;
            var actNegative = () => Discount.Create(negativePercentage);
            var actAbove100 = () => Discount.Create(invalidPercentage);

            actNegative.Should().Throw<DomainException>().WithMessage(ResourceMessages.DiscountMustBeBetweenZeroAndHundred);
            actAbove100.Should().Throw<DomainException>().WithMessage(ResourceMessages.DiscountMustBeBetweenZeroAndHundred);
        }

        [Fact]
        public void Given_DiscountObject_When_ImplicitlyConvertedToDecimal_Then_ShouldReturnValue()
        {
            var discount = Discount.Create(15.75m);

            decimal value = discount;

            value.Should().Be(15.75m);
        }

        [Fact]
        public void Given_DecimalValue_When_ImplicitlyConvertedToDiscount_Then_ShouldCreateDiscount()
        {
            decimal value = 30m;

            Discount discount = value;

            discount.Value.Should().Be(30m);
        }

        [Fact]
        public void Given_DiscountObject_When_ToStringCalled_Then_ShouldReturnFormattedValue()
        {
            var discount = Discount.Create(15.5m);

            var result = discount.ToString();

            result.Should().Be(15.5m.ToString("F2"));
        }
    }
}