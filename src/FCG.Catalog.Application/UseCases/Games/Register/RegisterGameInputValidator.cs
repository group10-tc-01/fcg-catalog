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
                .WithMessage(ResourceMessages.Name_Required)
                .MaximumLength(255)
                .WithMessage(ResourceMessages.Name_MaxLength);
            
            RuleFor(x => x.Price)
                .GreaterThan(0)
                .WithMessage(ResourceMessages.Price_Invalid);

            RuleFor(x => x.Category)
                .NotEmpty()
                .WithMessage(ResourceMessages.Category_Required)
                .Must(category => Enum.TryParse<GameCategory>(category, true, out _))
                .WithMessage(ResourceMessages.Category_Required);
        }
    }
}
