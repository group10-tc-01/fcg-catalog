using FCG.Catalog.Domain.Abstractions;
using System.Diagnostics.CodeAnalysis;

namespace FCG.Catalog.Domain.Catalog.Entities.Games
{
    [ExcludeFromCodeCoverage]
    public sealed class GameDetail : BaseEntity
    {
        public Guid GameId { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public decimal Price { get; private set; }
        public string Category { get; private set; } = string.Empty;
        public List<PromotionInfo> Promotions { get; private set; } = new();
        public SystemRequirementsInfo? SystemRequirements { get; private set; }
        public List<string> Tags { get; private set; } = new();
        public List<ScreenshotInfo> Screenshots { get; private set; } = new();
        public List<ReviewInfo> Reviews { get; private set; } = new();
        public int LibraryCount { get; private set; }
        public decimal AverageRating { get; private set; }
        public int TotalReviews { get; private set; }
        public DateTime SynchronizedAt { get; private set; }
        public int Version { get; private set; }

        private GameDetail() { }

        public GameDetail(
            Guid gameId,
            string title,
            string description,
            decimal price,
            string category,
            bool isActive,
            int libraryCount,
            decimal averageRating,
            int totalReviews,
            DateTime synchronizedAt,
            int version,
            List<PromotionInfo>? promotions = null,
            SystemRequirementsInfo? systemRequirements = null,
            List<string>? tags = null,
            List<ScreenshotInfo>? screenshots = null,
            List<ReviewInfo>? reviews = null)
        {
            GameId = gameId;
            Title = title;
            Description = description;
            Price = price;
            Category = category;
            IsActive = isActive;
            LibraryCount = libraryCount;
            AverageRating = averageRating;
            TotalReviews = totalReviews;
            SynchronizedAt = synchronizedAt;
            Version = version;
            Promotions = promotions ?? new();
            SystemRequirements = systemRequirements;
            Tags = tags ?? new();
            Screenshots = screenshots ?? new();
            Reviews = reviews ?? new();
        }

        public static GameDetail Create(
            Guid gameId,
            string title,
            string description,
            decimal price,
            string category,
            bool isActive)
        {
            return new GameDetail(
                gameId,
                title,
                description,
                price,
                category,
                isActive,
                libraryCount: 0,
                averageRating: 0m,
                totalReviews: 0,
                synchronizedAt: DateTime.UtcNow,
                version: 1);
        }
    }

    [ExcludeFromCodeCoverage]
    public class PromotionInfo
    {
        public Guid PromotionId { get; set; }
        public decimal DiscountPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class SystemRequirementsInfo
    {
        public string MinimumOs { get; set; } = string.Empty;
        public string MinimumProcessor { get; set; } = string.Empty;
        public string MinimumMemory { get; set; } = string.Empty;
        public string MinimumGraphics { get; set; } = string.Empty;
        public string MinimumStorage { get; set; } = string.Empty;
        public string RecommendedOs { get; set; } = string.Empty;
        public string RecommendedProcessor { get; set; } = string.Empty;
        public string RecommendedMemory { get; set; } = string.Empty;
        public string RecommendedGraphics { get; set; } = string.Empty;
        public string RecommendedStorage { get; set; } = string.Empty;
    }

    [ExcludeFromCodeCoverage]
    public class ScreenshotInfo
    {
        public Guid ScreenshotId { get; set; }
        public string Url { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public DateTime UploadedAt { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class ReviewInfo
    {
        public Guid ReviewId { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int Helpful { get; set; }
        public int NotHelpful { get; set; }
        public DateTime ReviewedAt { get; set; }
        public bool IsVerifiedPurchase { get; set; }
    }
}