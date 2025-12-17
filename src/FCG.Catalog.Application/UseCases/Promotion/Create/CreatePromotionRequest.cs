using System;
using MediatR;

namespace FCG.Catalog.Application.UseCases.Promotion.Create
{
    public class CreatePromotionRequest : IRequest<CreatePromotionResponse>
    {
        public Guid GameId { get; set; }
        public decimal DiscountPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
