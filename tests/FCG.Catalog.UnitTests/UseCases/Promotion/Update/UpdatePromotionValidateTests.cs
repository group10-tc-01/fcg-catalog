using FCG.Catalog.Application.UseCases.Promotion.Update;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace FCG.Catalog.UnitTests.UseCases.Promotion.Update;

public class UpdatePromotionValidateTests
{
    private readonly UpdatePromotionValidate _validator;

    public UpdatePromotionValidateTests()
    {
        _validator = new UpdatePromotionValidate();
    }

    [Fact]
    public void Validate_ShouldPass_WhenAllFieldsAreValid()
    {
        // Arrange
        var input = new UpdatePromotionInput
        {
            GameId = Guid.NewGuid(),
            DiscountPercentage = 50m,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(10)
        };

        // Act
        var result = _validator.Validate(input);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_ShouldFail_WhenGameIdIsEmpty()
    {
        // Arrange
        var input = new UpdatePromotionInput
        {
            GameId = Guid.Empty,
            DiscountPercentage = 50m,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(10)
        };

        // Act
        var result = _validator.TestValidate(input);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.GameId)
              .WithErrorMessage("The game name is required.");
    }

    [Fact]
    public void Validate_ShouldFail_WhenDiscountPercentageIsLessThan1()
    {
        // Arrange
        var input = new UpdatePromotionInput
        {
            GameId = Guid.NewGuid(),
            DiscountPercentage = 0m,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(10)
        };

        // Act
        var result = _validator.TestValidate(input);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DiscountPercentage)
              .WithErrorMessage("Discount must be between 0 and 100.");
    }

    [Fact]
    public void Validate_ShouldFail_WhenDiscountPercentageIsGreaterThan100()
    {
        // Arrange
        var input = new UpdatePromotionInput
        {
            GameId = Guid.NewGuid(),
            DiscountPercentage = 101m,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(10)
        };

        // Act
        var result = _validator.TestValidate(input);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DiscountPercentage)
              .WithErrorMessage("Discount must be between 0 and 100.");
    }

    [Fact]
    public void Validate_ShouldFail_WhenStartDateIsNotLessThanEndDate()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var input = new UpdatePromotionInput
        {
            GameId = Guid.NewGuid(),
            DiscountPercentage = 50m,
            StartDate = now.AddDays(10),
            EndDate = now.AddDays(1) // StartDate >= EndDate
        };

        // Act
        var result = _validator.TestValidate(input);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.StartDate)
              .WithErrorMessage("End date must be on or after the start date.");
    }

    [Fact]
    public void Validate_ShouldFail_WhenEndDateIsNotGreaterThanStartDate()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var input = new UpdatePromotionInput
        {
            GameId = Guid.NewGuid(),
            DiscountPercentage = 50m,
            StartDate = now.AddDays(10),
            EndDate = now.AddDays(1) // EndDate <= StartDate
        };

        // Act
        var result = _validator.TestValidate(input);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EndDate)
              .WithErrorMessage("End date must be on or after the start date.");
    }

    [Fact]
    public void Validate_ShouldFail_WhenMultipleRulesAreViolated()
    {
        // Arrange
        var input = new UpdatePromotionInput
        {
            GameId = Guid.Empty,
            DiscountPercentage = 150m,
            StartDate = DateTime.UtcNow.AddDays(10),
            EndDate = DateTime.UtcNow.AddDays(1)
        };

        // Act
        var result = _validator.TestValidate(input);

        // Assert
        result.Errors.Should().HaveCount(4); // GameId, DiscountPercentage, StartDate, EndDate
        result.ShouldHaveValidationErrorFor(x => x.GameId);
        result.ShouldHaveValidationErrorFor(x => x.DiscountPercentage);
        result.ShouldHaveValidationErrorFor(x => x.StartDate);
        result.ShouldHaveValidationErrorFor(x => x.EndDate);
    }
}