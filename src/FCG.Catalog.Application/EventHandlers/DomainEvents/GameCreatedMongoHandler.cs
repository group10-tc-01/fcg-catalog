using FCG.Catalog.Domain.Catalog.Events;
using FCG.Catalog.Domain.Catalog.Entities.Games;
using FCG.Catalog.Domain.Repositories.GameDetail;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace FCG.Catalog.Application.EventHandlers.DomainEvents
{
    [ExcludeFromCodeCoverage]
    public sealed class GameCreatedMongoHandler : INotificationHandler<GameCreatedEvent>
    {
        private readonly IWriteOnlyGameDetailRepository _gameDetailRepository;
        private readonly ILogger<GameCreatedMongoHandler> _logger;

        public GameCreatedMongoHandler(
            IWriteOnlyGameDetailRepository gameDetailRepository,
            ILogger<GameCreatedMongoHandler> logger)
        {
            _gameDetailRepository = gameDetailRepository ?? throw new ArgumentNullException(nameof(gameDetailRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(GameCreatedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Iniciando persistência de dados expandidos do jogo {GameId} no MongoDB",
                    notification.GameId);

                // ✅ Usar GameDetail da domain (não GameDetailDocument)
                var gameDetail = new GameDetail(
                    gameId: notification.GameId,
                    title: notification.Title,
                    description: notification.Description,
                    price: notification.Price,
                    category: notification.Category,
                    isActive: true,
                    libraryCount: 0,
                    averageRating: 0,
                    totalReviews: 0,
                    synchronizedAt: DateTime.UtcNow,
                    version: 1,
                    // Não implementado ainda, mas já inicializado para evitar nulls
                    promotions: new List<PromotionInfo>(),
                    systemRequirements: null,
                    tags: new List<string>(),
                    screenshots: new List<ScreenshotInfo>(),
                    reviews: new List<ReviewInfo>()
                );

                // ✅ Usar o repository existente com Upsert para garantir idempotência
                await _gameDetailRepository.AddOrUpdateAsync(gameDetail, cancellationToken);

                _logger.LogInformation(
                    "Dados expandidos do jogo {GameId} persistidos com sucesso no MongoDB",
                    notification.GameId);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Erro ao persistir dados expandidos do jogo {GameId} no MongoDB",
                    notification.GameId);
                // Não re-lançar para não quebrar a transação SQL
            }
        }
    }
}
