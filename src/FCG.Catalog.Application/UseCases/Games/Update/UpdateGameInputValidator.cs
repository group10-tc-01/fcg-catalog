using FCG.Catalog.Domain.Enum;
using FCG.Catalog.Messages;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCG.Catalog.Application.UseCases.Games.Update
{
    [ExcludeFromCodeCoverage]

    public class UpdateGameInputValidator : AbstractValidator<UpdateGameInput>
    {
        public UpdateGameInputValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage(ResourceMessages.GameNameIsRequired)
                .MaximumLength(255)
                .WithMessage(ResourceMessages.GameCategoryMaxLength);

            RuleFor(x => x.Price)
                .GreaterThan(0)
                .WithMessage(ResourceMessages.GamePriceMustBeGreaterThanZero);
        }
    }
}
