using FCG.Catalog.Domain;
﻿using FCG.Catalog.Domain.Abstractions;
using FCG.Catalog.Domain.Catalog.ValueObjects;
using FCG.Catalog.Domain.Exception;
using FCG.Catalog.Domain.Repositories.Game;
using FCG.Catalog.Domain.Repositories.Promotion;
using FCG.Domain.Repositories.PromotionRepository;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace FCG.Catalog.Application.UseCases.Promotion.Create
{
    [ExcludeFromCodeCoverage]
    public class CreatePromotionUseCase : ICreatePromotionUseCase
    {
        private readonly IReadOnlyGameRepository _readOnlyGameRepository;
        private readonly IReadOnlyPromotionRepository _readOnlyPromotionRepository;
        private readonly IWriteOnlyPromotionRepository _writeOnlyPromotionRepository;
        private readonly IUnitOfWork _unitOfWork;


        public CreatePromotionUseCase(
            IReadOnlyGameRepository readOnlyGameRepository,
            IReadOnlyPromotionRepository readOnlyPromotionRepository,
            IWriteOnlyPromotionRepository writeOnlyPromotionRepository,
            IUnitOfWork unitOfWork
        )
        {
            _readOnlyGameRepository = readOnlyGameRepository;
            _readOnlyPromotionRepository = readOnlyPromotionRepository;
            _writeOnlyPromotionRepository = writeOnlyPromotionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<CreatePromotionOutput> Handle(CreatePromotionInput request, CancellationToken cancellationToken)
        {
            var gameExists = await _readOnlyGameRepository.ExistsAsync(request.GameId, cancellationToken);

            if (!gameExists)
            {
                throw new NotFoundException("Game not found.");
            }

            var hasActivePromotion = await _readOnlyPromotionRepository.ExistsActivePromotionForGameAsync(
                request.GameId,
                request.StartDate,
                request.EndDate,
                cancellationToken);

            if (hasActivePromotion)
            {
                throw new DomainException("An active promotion already exists for this game in the specified period.");
            }

            var discount = Discount.Create(request.DiscountPercentage);
            var promotion = Domain.Catalog.Entity.Promotions.Promotion.Create(request.GameId, discount, request.StartDate, request.EndDate);
            await _writeOnlyPromotionRepository.AddAsync(promotion, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CreatePromotionOutput
            {
                Id = promotion.Id,
                GameId = promotion.GameId,
                Discount = promotion.DiscountPercentage.Value,
                StartDate = promotion.StartDate,
                EndDate = promotion.EndDate
            };
        }
    }
}
