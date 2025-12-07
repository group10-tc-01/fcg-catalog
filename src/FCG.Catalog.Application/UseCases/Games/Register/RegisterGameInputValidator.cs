using FCG.Catalog.Domain.Enum;
using FCG.Catalog.Messages;
using FluentValidation;

namespace FCG.Catalog.Application.UseCases.Games.Register
{
    public class RegisterGameInputValidator : AbstractValidator<RegisterGameInput>
    {
        public RegisterGameInputValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(ResourceMessages.GameNameIsRequired)
                .MaximumLength(255)
                .WithMessage(ResourceMessages.GameCategoryMaxLength);
            
            RuleFor(x => x.Price)
                .GreaterThan(0)
                .WithMessage(ResourceMessages.GamePriceMustBeGreaterThanZero);

            RuleFor(x => x.Category)
                .NotEmpty()
                .WithMessage(ResourceMessages.GameCategoryIsRequired)
                .Must(category => Enum.TryParse<GameCategory>(category, true, out _))
                .WithMessage(ResourceMessages.GameCategoryIsRequired);
        }
    }
}
