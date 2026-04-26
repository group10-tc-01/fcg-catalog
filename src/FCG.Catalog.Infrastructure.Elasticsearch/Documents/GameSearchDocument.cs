namespace FCG.Catalog.Infrastructure.Elasticsearch.Documents
{
    public sealed class GameSearchDocument
    {
        public string Id { get; init; } = string.Empty;

        public string Title { get; init; } = string.Empty;

        public string Description { get; init; } = string.Empty;

        public decimal Price { get; init; }

        public string Category { get; init; } = string.Empty;

        public decimal DiscountedPrice { get; init; }

        public bool IsActive { get; init; }

        public DateTime IndexedAt { get; init; }
    }
}
