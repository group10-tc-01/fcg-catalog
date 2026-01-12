using FCG.Catalog.Domain.Abstractions;
using FCG.Catalog.Domain.Exception;
using FCG.Catalog.Domain.Repositories.Promotion;
using System.Diagnostics.CodeAnalysis;

namespace FCG.Catalog.Application.UseCases.Promotion.Delete
{
    [ExcludeFromCodeCoverage]
    public class DeletePromotionUseCase : IDeletePromotionUseCase
    {
        private readonly IReadOnlyPromotionRepository _readOnlyPromotionRepository;
        private readonly IWriteOnlyPromotionRepository _writeOnlyPromotionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeletePromotionUseCase(
            IReadOnlyPromotionRepository readOnlyPromotionRepository,
            IWriteOnlyPromotionRepository writeOnlyPromotionRepository,
            IUnitOfWork unitOfWork)
        {
            _readOnlyPromotionRepository = readOnlyPromotionRepository;
            _writeOnlyPromotionRepository = writeOnlyPromotionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<DeletePromotionOutput> Handle(DeletePromotionInput request, CancellationToken cancellationToken)
        {
            var existing = await _readOnlyPromotionRepository.GetByIdAsync(request.Id, cancellationToken);

            if (existing is null)
            {
                throw new NotFoundException("Promotion not found.");
            }

            await _writeOnlyPromotionRepository.DeleteAsync(existing, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new DeletePromotionOutput { Id = request.Id };
        }
    }
}
