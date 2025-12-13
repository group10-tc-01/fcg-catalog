using FCG.Catalog.Domain.Catalog.ValueObjects;
using FCG.Catalog.Domain.Exception;
using FCG.Catalog.Messages;
using FluentAssertions;

namespace FCG.Catalog.UnitTests.Domain.ValueObjects
{
    public class NameTests
    {
        [Fact]
        public void Given_ValidName_When_Create_Then_ShouldCreateSuccessfully()
        {
            string validName = "FIFA";

            var name = Title.Create(validName);

            name.Should().NotBeNull();
            name.Value.Should().Be(validName);
        }

        [Fact]
        public void Given_ShortName_When_Create_Then_ShouldThrowDomainException()
        {
            string shortName = "A";
            var act = () => Title.Create(shortName);

            act.Should().Throw<DomainException>().WithMessage(ResourceMessages.GameTitleMinLength);
        }

        [Fact]
        public void Given_NullName_When_Create_Then_ShouldThrowDomainException()
        {
            string? nullName = null;
            var act = () => Title.Create(nullName!);

            act.Should().Throw<DomainException>().WithMessage(ResourceMessages.GameNameIsRequired);
        }

        [Fact]
        public void Given_EmptyName_When_Create_Then_ShouldThrowDomainException()
        {
            string emptyName = string.Empty;
            var act = () => Title.Create(emptyName);

            act.Should().Throw<DomainException>().WithMessage(ResourceMessages.GameNameIsRequired);
        }

        [Fact]
        public void Given_WhitespaceOnlyName_When_Create_Then_ShouldThrowDomainException()
        {
            string whitespaceName = "   ";
            var act = () => Title.Create(whitespaceName);

            act.Should().Throw<DomainException>().WithMessage(ResourceMessages.GameNameIsRequired);
        }

        [Fact]
        public void Given_ExactlyTwoCharactersName_When_Create_Then_ShouldCreateSuccessfully()
        {
            string minValidName = "AB";

            var act = () => Title.Create(minValidName);

            act.Should().Throw<DomainException>().WithMessage(ResourceMessages.GameTitleMinLength);

        }

        [Fact]
        public void Given_VeryLongName_When_Create_Then_ShouldCreateSuccessfully()
        {
            string longName = new string('A', 100);

            var name = Title.Create(longName);

            name.Should().NotBeNull();
            name.Value.Should().Be(longName);
        }

        [Fact]
        public void Given_NameWithSpecialCharacters_When_Create_Then_ShouldCreateSuccessfully()
        {
            string nameWithSpecialChars = "Call of Duty: Modern Warfare II";

            var name = Title.Create(nameWithSpecialChars);

            name.Should().NotBeNull();
            name.Value.Should().Be(nameWithSpecialChars);
        }

        [Fact]
        public void Given_NameWithNumbers_When_Create_Then_ShouldCreateSuccessfully()
        {
            string nameWithNumbers = "FIFA 2024";

            var name = Title.Create(nameWithNumbers);

            name.Should().NotBeNull();
            name.Value.Should().Be(nameWithNumbers);
        }

        [Fact]
        public void Given_NameObject_When_ToStringCalled_Then_ShouldReturnValue()
        {
            var name = Title.Create("Grand Theft Auto V");

            string stringValue = name.ToString();

            stringValue.Should().Be("Grand Theft Auto V");
        }

        [Fact]
        public void Given_StringValue_When_ImplicitConversionToName_Then_ShouldCreateName()
        {
            string stringValue = "TheBoy";

            Title name = stringValue;

            name.Should().NotBeNull();
            name.Value.Should().Be(stringValue);
        }

        [Fact]
        public void Given_NameObject_When_ImplicitConversionToString_Then_ShouldReturnValue()
        {
            var name = Title.Create("Cyberpunk");

            string stringValue = name;

            stringValue.Should().Be("Cyberpunk");
        }
    }
}
