using System.Diagnostics.CodeAnalysis;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FCG.Catalog.Infrastructure.MongoDb.Persistence.Entities
{
    [ExcludeFromCodeCoverage]
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

        [BsonElement("systemRequirements")]
        public SystemRequirements? SystemRequirements { get; set; }

        [BsonElement("tags")]
        public List<string> Tags { get; set; } = new();

        [BsonElement("screenshots")]
        public List<Screenshot> Screenshots { get; set; } = new();

        [BsonElement("reviews")]
        public List<Review> Reviews { get; set; } = new();

        [BsonElement("libraryCount")]
        public int LibraryCount { get; set; }

        [BsonElement("averageRating")]
        public decimal AverageRating { get; set; }

        [BsonElement("totalReviews")]
        public int TotalReviews { get; set; }

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

    [ExcludeFromCodeCoverage]
    public class SystemRequirements
    {
        [BsonElement("minimumOs")]
        public string MinimumOs { get; set; } = string.Empty;

        [BsonElement("minimumProcessor")]
        public string MinimumProcessor { get; set; } = string.Empty;

        [BsonElement("minimumMemory")]
        public string MinimumMemory { get; set; } = string.Empty;

        [BsonElement("minimumGraphics")]
        public string MinimumGraphics { get; set; } = string.Empty;

        [BsonElement("minimumStorage")]
        public string MinimumStorage { get; set; } = string.Empty;

        [BsonElement("recommendedOs")]
        public string RecommendedOs { get; set; } = string.Empty;

        [BsonElement("recommendedProcessor")]
        public string RecommendedProcessor { get; set; } = string.Empty;

        [BsonElement("recommendedMemory")]
        public string RecommendedMemory { get; set; } = string.Empty;

        [BsonElement("recommendedGraphics")]
        public string RecommendedGraphics { get; set; } = string.Empty;

        [BsonElement("recommendedStorage")]
        public string RecommendedStorage { get; set; } = string.Empty;
    }

    [ExcludeFromCodeCoverage]
    public class Screenshot
    {
        [BsonElement("screenshotId")]
        public Guid ScreenshotId { get; set; }

        [BsonElement("url")]
        public string Url { get; set; } = string.Empty;

        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;

        [BsonElement("displayOrder")]
        public int DisplayOrder { get; set; }

        [BsonElement("uploadedAt")]
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }

    [ExcludeFromCodeCoverage]
    public class Review
    {
        [BsonElement("reviewId")]
        public Guid ReviewId { get; set; }

        [BsonElement("userId")]
        public Guid UserId { get; set; }

        [BsonElement("userName")]
        public string UserName { get; set; } = string.Empty;

        [BsonElement("rating")]
        public int Rating { get; set; }

        [BsonElement("title")]
        public string Title { get; set; } = string.Empty;

        [BsonElement("content")]
        public string Content { get; set; } = string.Empty;

        [BsonElement("helpful")]
        public int Helpful { get; set; }

        [BsonElement("notHelpful")]
        public int NotHelpful { get; set; }

        [BsonElement("reviewedAt")]
        public DateTime ReviewedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("isVerifiedPurchase")]
        public bool IsVerifiedPurchase { get; set; }
    }
}
