using FCG.Catalog.Domain.Catalog.ValueObjects;
using FCG.Catalog.Domain.Exception;
using FCG.Catalog.Messages;
using FluentAssertions;

namespace FCG.Catalog.UnitTests.Domain.ValueObjects
{
    public class DiscountTests
    {
        [Fact]
        public void Given_ValidDiscount_When_Create_Then_ShouldCreateSuccessfully()
        {
            // Arrange
            decimal validDiscount = 15.5m;

            // Act
            var discount = Discount.Create(validDiscount);

            // Assert
            discount.Should().NotBeNull();
            discount.Value.Should().Be(validDiscount);
        }

        [Fact]
        public void Given_NegativeDiscount_When_Create_Then_ShouldThrowDomainException()
        {
            // Arrange
            decimal negativeDiscount = -5m;

            // Act
            var act = () => Discount.Create(negativeDiscount);

            // Assert
            act.Should().Throw<DomainException>().WithMessage(ResourceMessages.DiscountMustBeBetweenZeroAndHundred);
        }

        [Fact]
        public void Given_DiscountGreaterThanHundred_When_Create_Then_ShouldThrowDomainException()
        {
            // Arrange
            decimal invalidDiscount = 150m;

            // Act
            var act = () => Discount.Create(invalidDiscount);

            // Assert
            act.Should().Throw<DomainException>().WithMessage(ResourceMessages.DiscountMustBeBetweenZeroAndHundred);
        }

        [Fact]
        public void Given_ZeroDiscount_When_Create_Then_ShouldCreateSuccessfully()
        {
            // Arrange
            decimal zeroDiscount = 0m;

            // Act
            var discount = Discount.Create(zeroDiscount);

            // Assert
            discount.Value.Should().Be(zeroDiscount);
        }

        [Fact]
        public void Given_HundredDiscount_When_Create_Then_ShouldCreateSuccessfully()
        {
            // Arrange
            decimal hundredDiscount = 100m;

            // Act
            var discount = Discount.Create(hundredDiscount);

            // Assert
            discount.Value.Should().Be(hundredDiscount);
        }

        [Fact]
        public void Given_ImplicitConversionFromDecimal_When_Convert_Then_ShouldCreateDiscount()
        {
            // Arrange
            decimal value = 25m;

            // Act
            Discount discount = value;

            // Assert
            discount.Value.Should().Be(value);
        }

        [Fact]
        public void Given_ImplicitConversionToDecimal_When_Convert_Then_ShouldReturnValue()
        {
            // Arrange
            var discount = Discount.Create(30m);

            // Act
            decimal value = discount;

            // Assert
            value.Should().Be(discount.Value);
        }

        [Fact]
        public void Given_Discount_When_ToString_Then_ShouldFormatCorrectly()
        {
            // Arrange
            var discount = Discount.Create(12.345m);

            // Act
            var result = discount.ToString();

            // Assert
            result.Should().Be("12,35");
        }
    }
}