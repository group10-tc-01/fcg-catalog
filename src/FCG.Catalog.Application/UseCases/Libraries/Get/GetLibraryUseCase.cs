using FCG.Catalog.Domain.Repositories.Library;
using FCG.Catalog.Infrastructure.Redis.Interface;
using MediatR;

namespace FCG.Catalog.Application.UseCases.Libraries.Get
{
    public class GetLibraryUseCase : IRequestHandler<GetLibraryByUserIdQuery, GetLibraryByUserIdResponse>
    {
        private readonly IReadOnlyLibraryRepository _readOnlyLibraryRepository;

        public GetLibraryUseCase(IReadOnlyLibraryRepository readRepo, ICaching cache)
        {
            _readOnlyLibraryRepository = readRepo;
        }

        public async Task<GetLibraryByUserIdResponse> Handle(GetLibraryByUserIdQuery request, CancellationToken cancellationToken)
        {

            var library = await _readOnlyLibraryRepository.GetByUserIdWithGamesAsync(request.UserId, cancellationToken);

            if (library == null)
            {
                return new GetLibraryByUserIdResponse
                {
                    LibraryId = Guid.Empty,
                    LibraryGames = new List<LibraryGameDto>()
                };
            }

            var gamesDto = library.LibraryGames.Select(lg => new LibraryGameDto
            {
                GameId = lg.GameId,
                Title = lg.Game.Title.Value,
                Description = lg.Game.Description,
                PurchasePrice = lg.PurchasePrice.Value,
                PurchaseDate = lg.PurchaseDate
            })
                .OrderByDescending(g => g.PurchaseDate)
                .ToList();

            var response = new GetLibraryByUserIdResponse
            {
                LibraryId = library.Id,
                LibraryGames = gamesDto
            };


            return response;
        }
    }
}