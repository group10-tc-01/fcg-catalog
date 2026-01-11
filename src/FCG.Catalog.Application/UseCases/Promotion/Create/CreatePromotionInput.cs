using MediatR;
using System;
using System.Diagnostics.CodeAnalysis;

namespace FCG.Catalog.Application.UseCases.Promotion.Create
{
    [ExcludeFromCodeCoverage]
    public class CreatePromotionInput : IRequest<CreatePromotionOutput>
    {
        public Guid GameId { get; set; }
        public decimal DiscountPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
