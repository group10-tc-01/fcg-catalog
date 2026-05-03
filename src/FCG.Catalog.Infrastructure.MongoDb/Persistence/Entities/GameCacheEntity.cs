using FCG.Catalog.Infrastructure.MongoDb.Persistence.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace FCG.Catalog.Infrastructure.MongoDb.Persistence.Entities
{
    [ExcludeFromCodeCoverage]
    [BsonCollection("games_cache")]
    public class GameCacheEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        // Dados do jogo/game
        [BsonElement("gameId")]
        public Guid GameId { get; set; }

        [BsonElement("title")]
        public string Title { get; set; } = string.Empty;

        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;

        [BsonElement("price")]
        public decimal Price { get; set; }

        [BsonElement("category")]
        public string Category { get; set; } = string.Empty;

        [BsonElement("isActive")]
        public bool IsActive { get; set; } = true;

        // Dados de negócio
        [BsonElement("activePromotion")]
        public ActivePromotionCache? ActivePromotion { get; set; }

        [BsonElement("finalPrice")]
        public decimal FinalPrice { get; set; }

        // Dados de cache/acesso
        [BsonElement("accessCount")]
        public int AccessCount { get; set; }

        [BsonElement("cachedAt")]
        public DateTime CachedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("lastAccessedAt")]
        public DateTime LastAccessedAt { get; set; } = DateTime.UtcNow;
    }

    [ExcludeFromCodeCoverage]
    public class ActivePromotionCache
    {
        [BsonElement("promotionId")]
        public Guid PromotionId { get; set; }

        [BsonElement("discountPercentage")]
        public decimal DiscountPercentage { get; set; }

        [BsonElement("startDate")]
        public DateTime StartDate { get; set; }

        [BsonElement("endDate")]
        public DateTime EndDate { get; set; }
    }
}