using Xunit;
using FCG.Catalog.Domain.Catalog.ValueObjects;
using System;

namespace FCG.Catalog.Tests.Unit
{
    public class PriceTests
    {
        [Theory]
        [InlineData(1.0)]
        [InlineData(10.5)]
        public void Create_WithValidValue_ShouldReturnPrice(decimal value)
        {
            var price = Price.Create(value);
            Assert.Equal(value, price.Value);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Create_WithInvalidValue_ShouldThrow(decimal value)
        {
            Assert.ThrowsAny<Exception>(() => Price.Create(value));
        }
    }
}
