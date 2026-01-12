using MediatR;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace FCG.Catalog.Application.UseCases.Promotion.Update
{
    [ExcludeFromCodeCoverage]
    public class UpdatePromotionInput : IRequest<UpdatePromotionOutput>
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        public Guid GameId { get; set; }
        public decimal DiscountPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
