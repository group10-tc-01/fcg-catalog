using FCG.Catalog.Application.Behaviors;
using FCG.Catalog.Application.UseCases.Games.Search;
using FCG.Catalog.Domain.Models;
using FluentAssertions;
using FluentValidation;

namespace FCG.Catalog.UnitTests.Application.Behaviors
{
    public class ValidationBehaviorTests
    {
        [Fact]
        public async Task Handle_ShouldThrowValidationException_WhenRequestIsInvalid()
        {
            // Arrange
            var behavior = new ValidationBehavior<SearchGamesInput, PagedListResponse<SearchGameOutput>>(
                new[] { new SearchGamesInputValidator() });

            var request = new SearchGamesInput
            {
                Term = string.Empty,
                PageNumber = 0,
                PageSize = 10
            };

            // Act
            var act = async () => await behavior.Handle(
                request,
                _ => Task.FromResult(new PagedListResponse<SearchGameOutput>([], 0, 1, 10)),
                CancellationToken.None);

            // Assert
            var exception = await act.Should().ThrowAsync<ValidationException>();
            exception.Which.Errors.Should().Contain(error => error.PropertyName == nameof(SearchGamesInput.Term));
            exception.Which.Errors.Should().Contain(error => error.PropertyName == nameof(SearchGamesInput.PageNumber));
        }

        [Fact]
        public async Task Handle_ShouldCallNext_WhenRequestIsValid()
        {
            // Arrange
            var behavior = new ValidationBehavior<SearchGamesInput, PagedListResponse<SearchGameOutput>>(
                new[] { new SearchGamesInputValidator() });

            var request = new SearchGamesInput
            {
                Term = "halo",
                PageNumber = 1,
                PageSize = 10
            };

            var wasCalled = false;

            // Act
            await behavior.Handle(
                request,
                _ =>
                {
                    wasCalled = true;
                    return Task.FromResult(new PagedListResponse<SearchGameOutput>([], 0, 1, 10));
                },
                CancellationToken.None);

            // Assert
            wasCalled.Should().BeTrue();
        }
    }
}
