using FCG.Catalog.Domain.Catalog.Entities.Games;
using FCG.Catalog.Domain.Repositories.GameDetail;
using FCG.Catalog.Infrastructure.MongoDb.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using System.Diagnostics.CodeAnalysis;

namespace FCG.Catalog.Infrastructure.MongoDb.Repositories
{
    [ExcludeFromCodeCoverage]
    public class GameDetailRepository : IReadOnlyGameDetailRepository, IWriteOnlyGameDetailRepository
    {
        private readonly MongoDbContext _context;

        public GameDetailRepository(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<GameDetail?> GetByIdAsync(Guid gameId, CancellationToken cancellationToken = default)
        {
            var document = await _context.GamesDetail
                .Where(g => g.GameId == gameId)
                .FirstOrDefaultAsync(cancellationToken);

            return document is null ? null : MapToEntity(document);
        }

        public async Task<bool> ExistsAsync(Guid gameId, CancellationToken cancellationToken = default)
        {
            var document = await GetByIdAsync(gameId, cancellationToken);
            return document is not null;
        }

        public IQueryable<GameDetail> GetAll()
        {
            return _context.GamesDetail.AsQueryable().Select(d => MapToEntity(d));
        }

        public async Task AddAsync(GameDetail gameDetail, CancellationToken cancellationToken = default)
        {
            var document = MapToDocument(gameDetail);
            await _context.GamesDetail.AddAsync(document, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(GameDetail gameDetail, CancellationToken cancellationToken = default)
        {
            var document = MapToDocument(gameDetail);
            _context.GamesDetail.Update(document);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task AddOrUpdateAsync(GameDetail gameDetail, CancellationToken cancellationToken = default)
        {
            var document = MapToDocument(gameDetail);

            // Verifica se já existe para fazer Upsert
            var existingDocument = await _context.GamesDetail
                .Where(g => g.GameId == gameDetail.GameId) // Importante: use o GameId ou Id conforme mapeado
                .FirstOrDefaultAsync(cancellationToken);

            if (existingDocument is null)
            {
                await _context.GamesDetail.AddAsync(document, cancellationToken);
            }
            else
            {
                document.GameId = existingDocument.GameId;

                _context.Entry(existingDocument).CurrentValues.SetValues(document);
                // Ou se não tiver Tracking ativo: _context.GamesDetail.Update(document);
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Guid gameId, CancellationToken cancellationToken = default)
        {
            var document = await _context.GamesDetail
                .Where(g => g.GameId == gameId)
                .FirstOrDefaultAsync(cancellationToken);

            if (document is not null)
            {
                _context.GamesDetail.Remove(document);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        private static GameDetail MapToEntity(GameDetailDocument document)
        {
            var promotions = document.Promotions?.Select(p => new PromotionInfo
            {
                PromotionId = p.PromotionId,
                DiscountPercentage = p.DiscountPercentage,
                StartDate = p.StartDate,
                EndDate = p.EndDate
            }).ToList();

            var systemRequirements = document.SystemRequirements is null ? null : new SystemRequirementsInfo
            {
                MinimumOs = document.SystemRequirements.MinimumOs,
                MinimumProcessor = document.SystemRequirements.MinimumProcessor,
                MinimumMemory = document.SystemRequirements.MinimumMemory,
                MinimumGraphics = document.SystemRequirements.MinimumGraphics,
                MinimumStorage = document.SystemRequirements.MinimumStorage,
                RecommendedOs = document.SystemRequirements.RecommendedOs,
                RecommendedProcessor = document.SystemRequirements.RecommendedProcessor,
                RecommendedMemory = document.SystemRequirements.RecommendedMemory,
                RecommendedGraphics = document.SystemRequirements.RecommendedGraphics,
                RecommendedStorage = document.SystemRequirements.RecommendedStorage
            };

            var tags = document.Tags ?? new();

            var screenshots = document.Screenshots?.Select(s => new ScreenshotInfo
            {
                ScreenshotId = s.ScreenshotId,
                Url = s.Url,
                Description = s.Description,
                DisplayOrder = s.DisplayOrder,
                UploadedAt = s.UploadedAt
            }).ToList();

            var reviews = document.Reviews?.Select(r => new ReviewInfo
            {
                ReviewId = r.ReviewId,
                UserId = r.UserId,
                UserName = r.UserName,
                Rating = r.Rating,
                Title = r.Title,
                Content = r.Content,
                Helpful = r.Helpful,
                NotHelpful = r.NotHelpful,
                ReviewedAt = r.ReviewedAt,
                IsVerifiedPurchase = r.IsVerifiedPurchase
            }).ToList();

            return new GameDetail(
                document.GameId,
                document.Title,
                document.Description,
                document.Price,
                document.Category,
                document.IsActive,
                document.LibraryCount,
                document.AverageRating,
                document.TotalReviews,
                document.SynchronizedAt,
                document.Version,
                promotions,
                systemRequirements,
                tags,
                screenshots,
                reviews);
        }

        private static GameDetailDocument MapToDocument(GameDetail entity)
        {
            return new GameDetailDocument
            {
                GameId = entity.GameId,
                Title = entity.Title,
                Description = entity.Description,
                Price = entity.Price,
                Category = entity.Category,
                IsActive = entity.IsActive,
                Promotions = entity.Promotions?.Select(p => new PromotionDetail
                {
                    PromotionId = p.PromotionId,
                    DiscountPercentage = p.DiscountPercentage,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate
                }).ToList() ?? new(),
                SystemRequirements = entity.SystemRequirements is null ? null : new SystemRequirements
                {
                    MinimumOs = entity.SystemRequirements.MinimumOs,
                    MinimumProcessor = entity.SystemRequirements.MinimumProcessor,
                    MinimumMemory = entity.SystemRequirements.MinimumMemory,
                    MinimumGraphics = entity.SystemRequirements.MinimumGraphics,
                    MinimumStorage = entity.SystemRequirements.MinimumStorage,
                    RecommendedOs = entity.SystemRequirements.RecommendedOs,
                    RecommendedProcessor = entity.SystemRequirements.RecommendedProcessor,
                    RecommendedMemory = entity.SystemRequirements.RecommendedMemory,
                    RecommendedGraphics = entity.SystemRequirements.RecommendedGraphics,
                    RecommendedStorage = entity.SystemRequirements.RecommendedStorage
                },
                Tags = entity.Tags ?? new(),
                Screenshots = entity.Screenshots?.Select(s => new Screenshot
                {
                    ScreenshotId = s.ScreenshotId,
                    Url = s.Url,
                    Description = s.Description,
                    DisplayOrder = s.DisplayOrder,
                    UploadedAt = s.UploadedAt
                }).ToList() ?? new(),
                Reviews = entity.Reviews?.Select(r => new Review
                {
                    ReviewId = r.ReviewId,
                    UserId = r.UserId,
                    UserName = r.UserName,
                    Rating = r.Rating,
                    Title = r.Title,
                    Content = r.Content,
                    Helpful = r.Helpful,
                    NotHelpful = r.NotHelpful,
                    ReviewedAt = r.ReviewedAt,
                    IsVerifiedPurchase = r.IsVerifiedPurchase
                }).ToList() ?? new(),
                LibraryCount = entity.LibraryCount,
                AverageRating = entity.AverageRating,
                TotalReviews = entity.TotalReviews,
                SynchronizedAt = entity.SynchronizedAt,
                Version = entity.Version
            };
        }
    }
}
