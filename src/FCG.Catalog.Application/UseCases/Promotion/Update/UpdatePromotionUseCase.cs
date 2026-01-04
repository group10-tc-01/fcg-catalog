using FCG.Catalog.Domain;
using FCG.Catalog.Domain.Abstractions;
using FCG.Catalog.Domain.Catalog.ValueObjects;
using FCG.Catalog.Domain.Exception;
using FCG.Catalog.Domain.Repositories.Promotion;
using FCG.Domain.Repositories.PromotionRepository;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace FCG.Catalog.Application.UseCases.Promotion.Update
{
    [ExcludeFromCodeCoverage]
    public class UpdatePromotionUseCase : IUpdatePromotionUseCase
    {
        private readonly IReadOnlyPromotionRepository _readOnlyPromotionRepository;
        private readonly IWriteOnlyPromotionRepository _writeOnlyPromotionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdatePromotionUseCase(
            IReadOnlyPromotionRepository readOnlyPromotionRepository,
            IWriteOnlyPromotionRepository writeOnlyPromotionRepository,
            IUnitOfWork unitOfWork)
        {
            _readOnlyPromotionRepository = readOnlyPromotionRepository;
            _writeOnlyPromotionRepository = writeOnlyPromotionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<UpdatePromotionOutput> Handle(UpdatePromotionInput request, CancellationToken cancellationToken)
        {
            var existing = await _readOnlyPromotionRepository.GetByIdAsync(request.Id, cancellationToken);

            if (existing is null)
            {
                throw new NotFoundException("Promotion not found.");
            }

            var discount = Discount.Create(request.DiscountPercentage);

            var hasOverlap = await _readOnlyPromotionRepository.ExistsActivePromotionForGameAsync(
                request.GameId,
                request.StartDate,
                request.EndDate,
                cancellationToken);

            if (hasOverlap && existing.GameId != request.GameId)
            {
                throw new DomainException("An active promotion already exists for this game in the specified period.");
            }

            existing.Update(request.GameId, discount, request.StartDate, request.EndDate);
            await _writeOnlyPromotionRepository.UpdateAsync(existing, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new UpdatePromotionOutput
            {
                Id = existing.Id,
                GameId = existing.GameId,
                Discount = existing.DiscountPercentage.Value,
                StartDate = existing.StartDate,
                EndDate = existing.EndDate
            };
        }
    }
}
