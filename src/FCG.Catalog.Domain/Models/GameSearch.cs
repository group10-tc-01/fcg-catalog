using System.Diagnostics.CodeAnalysis;

namespace FCG.Catalog.Domain.Models
{
    [ExcludeFromCodeCoverage]
    public sealed class GameSearch
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public string Category { get; set; } = string.Empty;

        public decimal DiscountedPrice { get; set; }

        public bool IsActive { get; set; }

        public DateTime IndexedAt { get; set; }

        public double Score { get; set; }
    }
}
