using FCG.Catalog.Domain.Exception;
using FCG.Catalog.Domain.Repositories.Game;
using FCG.Catalog.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCG.Catalog.Application.UseCases.Games.GetById
{
    public class GetGameIdUseCase : IGetGameIdUseCase
    {
        private readonly IReadOnlyGameRepository _gameRepository;
        public GetGameIdUseCase(IReadOnlyGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }
        public async Task<GetGameIdOutput?> Handle(GetGameIdInput input, CancellationToken cancellationToken)
        {
            var game = await _gameRepository.GetByIdAsync(input.Id, cancellationToken);

            if (game is null)
                throw new NotFoundException(ResourceMessages.GameNotFound);

            return new GetGameIdOutput
            {
                Title = game.Title,
                Description = game.Description,
                Price = game.Price,
                Category =  game.Category
            };
        }
    }
}
