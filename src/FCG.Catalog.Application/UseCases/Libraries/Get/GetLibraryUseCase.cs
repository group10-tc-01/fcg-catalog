using FCG.Catalog.Domain.Catalog.Entities.LibraryGames;
using FCG.Catalog.Domain.Repositories.Library;
using FCG.Domain.Repositories.LibraryRepository;
using MediatR;

namespace FCG.Catalog.Application.UseCases.Libraries.Get
{
    public class GetLibraryUseCase : IRequestHandler<GetLibraryByUserIdQuery, GetLibraryByUserIdResponse>
    {
        private readonly IReadOnlyLibraryRepository _readOnlyLibraryRepository;

        public GetLibraryUseCase(IReadOnlyLibraryRepository readRepo)
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
                    LibraryGames = new List<LibraryGame>()
                };
            }

            return new GetLibraryByUserIdResponse
            {
                LibraryId = library.Id,
                LibraryGames = library.LibraryGames?.ToList()
            };
        }
    }
}
