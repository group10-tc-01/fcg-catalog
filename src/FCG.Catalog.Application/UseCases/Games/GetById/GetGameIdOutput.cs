using System.Diagnostics.CodeAnalysis;

namespace FCG.Catalog.Application.UseCases.Games.GetById
{
    [ExcludeFromCodeCoverage]
    public class GetGameIdOutput
    {
        public string Title { get; init; }
        public string Description { get; init; } = string.Empty;
        public string Category { get; init; }
        public decimal OriginalPrice { get; init; }
        public decimal? DiscountedPrice { get; init; }
        public bool HasActivePromotion { get; init; }
    }
}
