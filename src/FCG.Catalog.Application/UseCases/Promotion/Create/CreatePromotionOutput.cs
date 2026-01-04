using System;
using System.Diagnostics.CodeAnalysis;

namespace FCG.Catalog.Application.UseCases.Promotion.Create
{
    [ExcludeFromCodeCoverage]
    public class CreatePromotionOutput
    {
        public Guid Id { get; set; }
        public Guid GameId { get; set; }
        public decimal Discount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
