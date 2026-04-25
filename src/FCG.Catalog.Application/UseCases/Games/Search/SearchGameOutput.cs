using System.Diagnostics.CodeAnalysis;

namespace FCG.Catalog.Application.UseCases.Games.Search
{
    [ExcludeFromCodeCoverage]
    public class SearchGameOutput
    {
        public Guid Id { get; init; }

        public string Title { get; init; } = string.Empty;

        public string Description { get; init; } = string.Empty;

        public decimal Price { get; init; }

        public string Category { get; init; } = string.Empty;

        public decimal DiscountedPrice { get; init; }

        public bool IsActive { get; init; }

        public DateTime IndexedAt { get; init; }

        public double Score { get; init; }
    }
}
