using FCG.Catalog.Domain.Catalog.Entities.Games;
using FCG.Catalog.Domain.Models;

namespace FCG.Catalog.Application.Services
{
    internal static class GameSearchMapper
    {
        public static GameSearch ToGameSearch(Game game)
        {
            var activePromotion = game.GetActivePromotion();

            return new GameSearch
            {
                Id = game.Id,
                Title = game.Title.Value,
                Description = game.Description,
                Price = game.Price.Value,
                Category = game.Category.ToString(),
                DiscountedPrice = game.CalculateDiscountedPrice(activePromotion),
                IsActive = game.IsActive,
                IndexedAt = DateTime.UtcNow
            };
        }
    }
}
