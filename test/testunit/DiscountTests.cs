using Xunit;
using FCG.Catalog.Domain.Catalog.ValueObjects;
using System;

namespace FCG.Catalog.Tests.Unit
{
    public class DiscountTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(50)]
        [InlineData(100)]
        public void Create_WithValidValue_ShouldReturnDiscount(decimal value)
        {
            var discount = Discount.Create(value);
            Assert.Equal(value, discount.Value);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(101)]
        public void Create_WithInvalidValue_ShouldThrow(decimal value)
        {
            Assert.ThrowsAny<Exception>(() => Discount.Create(value));
        }
    }
}
