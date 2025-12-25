using FCG.Catalog.Domain.Catalog.Entity.Promotions;
using FCG.Catalog.Domain.Catalog.ValueObjects;
using FCG.Catalog.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCG.Catalog.Application.UseCases.Games.GetById
{
    [ExcludeFromCodeCoverage]
    public class GetGameIdOutput
    {
        public string Title { get; init; }
        public string Description { get; init; } = string.Empty;
        public GameCategory Category { get; init; }
        public decimal OriginalPrice { get; init; }
        public decimal? DiscountedPrice { get; init; }
        public bool HasActivePromotion { get; init; }
    }
}
