using FluentValidation;
using System.Diagnostics.CodeAnalysis;

namespace FCG.Catalog.Application.UseCases.Promotion.Create
{
    [ExcludeFromCodeCoverage]
    public class CreatePromotionInputValidator : AbstractValidator<CreatePromotionInput>
    {
        public CreatePromotionInputValidator()
        {
            RuleFor(x => x.GameId)
                .NotEmpty()
                .WithMessage("Game ID is required.");

            RuleFor(x => x.DiscountPercentage)
                .GreaterThan(0)
                .WithMessage("Discount percentage must be greater than 0.")
                .LessThanOrEqualTo(100)
                .WithMessage("Discount percentage must be less than or equal to 100.");

            RuleFor(x => x.StartDate)
                .NotEmpty()
                .WithMessage("Start date is required.")
                .Must(BeValidStartDate)
                .WithMessage("Start date cannot be in the past.");

            RuleFor(x => x.EndDate)
                .NotEmpty()
                .WithMessage("End date is required.")
                .GreaterThan(x => x.StartDate)
                .WithMessage("End date must be after the start date.");
        }

        private bool BeValidStartDate(DateTime startDate)
        {
            return startDate.Date >= DateTime.UtcNow.Date;
        }
    }
}
