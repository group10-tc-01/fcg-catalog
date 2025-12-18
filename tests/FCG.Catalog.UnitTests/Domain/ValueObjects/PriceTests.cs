using FCG.Catalog.Domain.Catalog.ValueObjects;
using FCG.Catalog.Domain.Exception;
using FCG.Catalog.Messages;
using FluentAssertions;

namespace FCG.Catalog.UnitTests.Domain.ValueObjects
{
    public class PriceTests
    {
        [Fact]
        public void Given_ValidPrice_When_Create_Then_ShouldCreateSuccessfully()
        {
            decimal validPrice = 59.99m;

            var price = Price.Create(validPrice);

            price.Should().NotBeNull();
            price.Value.Should().Be(validPrice);
        }

        [Fact]
        public void Given_VerySmallPrice_When_Create_Then_ShouldCreateSuccessfully()
        {
            decimal smallPrice = 0.01m;

            var price = Price.Create(smallPrice);

            price.Value.Should().Be(0.01m);
        }

        [Fact]
        public void Given_LargePrice_When_Create_Then_ShouldCreateSuccessfully()
        {
            decimal largePrice = decimal.MaxValue;

            var price = Price.Create(largePrice);

            price.Value.Should().Be(decimal.MaxValue);
        }

        [Fact]
        public void Given_NegativePrice_When_Create_Then_ShouldThrowDomainException()
        {
            decimal negativePrice = -10.50m;
            var act = () => Price.Create(negativePrice);

            act.Should().Throw<DomainException>().WithMessage(ResourceMessages.PriceCannotBeNegative);
        }

        [Fact]
        public void Given_PriceObject_When_ImplicitConvertToDecimal_Then_ShouldReturnValue()
        {
            var price = Price.Create(29.99m);

            decimal value = price;

            value.Should().Be(29.99m);
        }

        [Fact]
        public void Given_DecimalValue_When_ImplicitConvertToPrice_Then_ShouldCreatePrice()
        {
            decimal value = 49.99m;

            Price price = value;

            price.Value.Should().Be(49.99m);
        }

        [Fact]
        public void Given_PriceWithHighPrecision_When_Create_Then_ShouldMaintainPrecision()
        {
            decimal precisePrice = 19.999999m;

            var price = Price.Create(precisePrice);

            price.Value.Should().Be(19.999999m);
        }

        [Fact]
        public void Given_ZeroPrice_When_Create_Then_ShouldThrowDomainException()
        {
            decimal zeroPrice = 0m;
            var act = () => Price.Create(zeroPrice);

            act.Should().Throw<DomainException>().WithMessage(ResourceMessages.GamePriceMustBeGreaterThanZero);
        }
        [Fact]
        public void Given_PriceObject_When_ToStringCalled_Then_ShouldReturnFormattedValue()
        {
            var price = Price.Create(10.5m);

            var result = price.ToString();

            result.Should().Be(10.5m.ToString("F2"));
        }
        [Fact]
        public void Given_InvalidDecimalValue_When_ImplicitlyConvertedToPrice_Then_ShouldThrowException()
        {
            decimal invalidValue = -10m;

            Action act = () => { Price p = invalidValue; };

            act.Should().Throw<DomainException>()
                .WithMessage(ResourceMessages.PriceCannotBeNegative);
        }
    }
}
