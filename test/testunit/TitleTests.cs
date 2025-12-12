using Xunit;
using FCG.Catalog.Domain.Catalog.ValueObjects;
using System;

namespace FCG.Catalog.Tests.Unit
{
    public class TitleTests
    {
        [Fact]
        public void Create_WithValidValue_ShouldReturnTitle()
        {
            var title = Title.Create("Valid Title");
            Assert.Equal("Valid Title", title.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Create_WithNullOrWhitespace_ShouldThrow(string value)
        {
            Assert.ThrowsAny<Exception>(() => Title.Create(value!));
        }

        [Fact]
        public void Create_WithTooShort_ShouldThrow()
        {
            Assert.ThrowsAny<Exception>(() => Title.Create("A"));
        }

        [Fact]
        public void Create_WithTooLong_ShouldThrow()
        {
            var longStr = new string('a', 256);
            Assert.ThrowsAny<Exception>(() => Title.Create(longStr));
        }
    }
}
