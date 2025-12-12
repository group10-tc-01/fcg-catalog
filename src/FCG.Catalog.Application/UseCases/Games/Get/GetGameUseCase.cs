using FCG.Catalog.Domain.Repositories.Game;
using FCG.Catalog.Messages;
using MediatR;

namespace FCG.Catalog.Application.UseCases.Games.Get
{
    public class GetGameUseCase : IRequestHandler<GetGameInput, GetGameOutput>
    {
        private readonly IReadOnlyGameRepository _readRepo;

        public GetGameUseCase(IReadOnlyGameRepository readRepo)
        {
            _readRepo = readRepo;
        }

        public async Task<GetGameOutput> Handle(GetGameInput request, CancellationToken cancellationToken)
        {
            var game = await _readRepo.GetByIdAsync(request.Id, cancellationToken);
            if (game is null)
                throw new FCG.Catalog.Domain.Exception.NotFoundException(ResourceMessages.GameNotFound);

            return new GetGameOutput
            {
                Id = game.Id,
                Title = game.Title,
                Description = game.Description,
                Price = game.Price.Value,
                Category = game.Category.ToString(),
                IsActive = game.IsActive
            };
        }
    }
}
