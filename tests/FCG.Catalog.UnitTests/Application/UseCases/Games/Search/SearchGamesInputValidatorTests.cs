using FCG.Catalog.Application.UseCases.Games.Search;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace FCG.Catalog.UnitTests.Application.UseCases.Games.Search
{
    public class SearchGamesInputValidatorTests
    {
        private readonly SearchGamesInputValidator _validator = new();

        [Fact]
        public void Validate_ShouldPass_WhenPaginationAndTermAreValid()
        {
            // Arrange
            var input = new SearchGamesInput
            {
                Term = "rpg",
                PageNumber = 1,
                PageSize = 10
            };

            // Act
            var result = _validator.Validate(input);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Validate_ShouldFail_WhenTermIsEmpty(string term)
        {
            var input = new SearchGamesInput
            {
                Term = term,
                PageNumber = 1,
                PageSize = 10
            };

            var result = _validator.TestValidate(input);

            result.ShouldHaveValidationErrorFor(x => x.Term);
        }

        [Theory]
        [InlineData(0, 10)]
        [InlineData(1, 0)]
        [InlineData(1, 51)]
        public void Validate_ShouldFail_WhenPaginationIsInvalid(int pageNumber, int pageSize)
        {
            var input = new SearchGamesInput
            {
                Term = "action",
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = _validator.TestValidate(input);

            result.IsValid.Should().BeFalse();
        }
    }
}
