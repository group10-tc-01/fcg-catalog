using FCG.Catalog.Domain.Enum;
using FCG.Catalog.Messages;
using FluentValidation;
using System;
using System.Diagnostics.CodeAnalysis;

namespace FCG.Catalog.Application.UseCases.Games.Register
{
    [ExcludeFromCodeCoverage]
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
                .WithMessage(ResourceMessages.GameCategoryIsRequired);
        }
    }
}
