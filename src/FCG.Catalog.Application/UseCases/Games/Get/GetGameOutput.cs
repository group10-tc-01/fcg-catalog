using System;

namespace FCG.Catalog.Application.UseCases.Games.Get
{ 
    public class GetGameOutput
    {
        public Guid Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public decimal Price { get; init; }
        public string Category { get; init; } = string.Empty;
        public bool IsActive { get; init; }
        public ActivePromotionDto? ActivePromotion { get; set; }
        public decimal FinalPrice { get; set; }

    }
    public class ActivePromotionDto
    {
        public Guid PromotionId { get; set; }
        public decimal DiscountPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
