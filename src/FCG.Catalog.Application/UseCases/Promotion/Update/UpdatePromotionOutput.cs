using System.Diagnostics.CodeAnalysis;

namespace FCG.Catalog.Application.UseCases.Promotion.Update
{
    [ExcludeFromCodeCoverage]
    public class UpdatePromotionOutput
    {
        public Guid Id { get; set; }
        public Guid GameId { get; set; }
        public decimal Discount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
