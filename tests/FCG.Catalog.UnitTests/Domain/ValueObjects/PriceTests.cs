using FCG.Catalog.Domain.Catalog.ValueObjects;
using FCG.Catalog.Domain.Exception;
using FCG.Catalog.Messages;
using FluentAssertions;
using System.Globalization;

namespace FCG.Catalog.UnitTests.Domain.ValueObjects
{
    public class PriceTests
    {
        [Fact]
        public void Given_ValidPrice_When_Create_Then_ShouldCreateSuccessfully()
        {
            // Arrange
            decimal validPrice = 59.99m;

            // Act
            var price = Price.Create(validPrice);

            // Assert
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
            // Arrange
            decimal negativePrice = -10.00m;

            // Act
            var act = () => Price.Create(negativePrice);

            // Assert
            act.Should().Throw<DomainException>().WithMessage(ResourceMessages.GamePriceMustBeGreaterThanZero);
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
            // Arrange
            decimal zeroPrice = 0m;

            // Act
            var act = () => Price.Create(zeroPrice);

            // Assert
            act.Should().Throw<DomainException>().WithMessage(ResourceMessages.GamePriceMustBeGreaterThanZero);
        }
        [Fact]
        public void Given_PriceObject_When_ToStringCalled_Then_ShouldReturnFormattedValue()
        {
            // Arrange
            var price = Price.Create(123.456m);

            // Act
            var result = price.Value.ToString("F2", CultureInfo.InvariantCulture);

            // Assert
            result.Should().Be("123.46");
        }
    }
}
