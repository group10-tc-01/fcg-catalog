using FCG.Catalog.Infrastructure.MongoDb.Persistence.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace FCG.Catalog.Infrastructure.MongoDb.Persistence.Entities
{
    [ExcludeFromCodeCoverage]
    [BsonCollection("games_detail")]
    public class GameDetailDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
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
        public bool IsActive { get; set; }

        [BsonElement("promotions")]
        public List<PromotionDetail> Promotions { get; set; } = new();

        [BsonElement("libraryCount")]
        public int LibraryCount { get; set; }

        [BsonElement("synchronizedAt")]
        public DateTime SynchronizedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("version")]
        public int Version { get; set; } = 1;
    }

    [ExcludeFromCodeCoverage]
    public class PromotionDetail
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