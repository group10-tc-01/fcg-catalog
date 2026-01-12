using MediatR;

namespace FCG.Catalog.Application.UseCases.Games.ProcessPayment
{
    public class ProcessPaymentResultInput : IRequest
    {
        public Guid CorrelationId { get; set; }
        public Guid UserId { get; set; }
        public Guid GameId { get; set; }
        public decimal Amount { get; set; }
        public bool IsApproved { get; set; }
    }
}
