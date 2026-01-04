using MediatR;
using System;
using System.Diagnostics.CodeAnalysis;

namespace FCG.Catalog.Application.UseCases.Promotion.Delete
{
    [ExcludeFromCodeCoverage]
    public class DeletePromotionInput : IRequest<DeletePromotionOutput>
    {
        public Guid Id { get; set; }
    }
}
