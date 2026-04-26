using FluentValidation;
using System.Diagnostics.CodeAnalysis;

namespace FCG.Catalog.Application.UseCases.Games.Search
{
    [ExcludeFromCodeCoverage]
    public class SearchGamesInputValidator : AbstractValidator<SearchGamesInput>
    {
        public SearchGamesInputValidator()
        {
            RuleFor(x => x.Term)
                .NotEmpty()
                .WithMessage("Search term is required.");

            RuleFor(x => x.PageNumber)
                .GreaterThan(0)
                .WithMessage("PageNumber deve ser maior que 0");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 50)
                .WithMessage("PageSize deve estar entre 1 e 50");
        }
    }
}
