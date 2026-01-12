using FCG.Catalog.Messages;
using FluentValidation;

namespace FCG.Catalog.Application.UseCases.Promotion.Update
{
    public class UpdatePromotionValidate : AbstractValidator<UpdatePromotionInput>
    {
        public UpdatePromotionValidate()
        {
            RuleFor(x => x.GameId)
                .NotEmpty().WithMessage(ResourceMessages.GameNameIsRequired);

            RuleFor(x => x.DiscountPercentage)
                .InclusiveBetween(1, 100).WithMessage(ResourceMessages.DiscountMustBeBetweenZeroAndHundred);

            RuleFor(x => x.StartDate)
                .LessThan(x => x.EndDate).WithMessage(ResourceMessages.PromotionEndDateMustBeAfterStartDate);

            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate).WithMessage(ResourceMessages.PromotionEndDateMustBeAfterStartDate);
        }
    }
}
