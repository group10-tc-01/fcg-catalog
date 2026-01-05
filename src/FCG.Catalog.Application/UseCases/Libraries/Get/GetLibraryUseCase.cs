using FCG.Catalog.Domain.Repositories.Library;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System.Reflection.PortableExecutable;
using System.Text.Json;
using FCG.Catalog.Infrastructure.Redis.Interface;

namespace FCG.Catalog.Application.UseCases.Libraries.Get
{
    public class GetLibraryUseCase : IRequestHandler<GetLibraryByUserIdQuery, GetLibraryByUserIdResponse>
    {
        private readonly IReadOnlyLibraryRepository _readOnlyLibraryRepository;
        private readonly ICaching _cache;

        public GetLibraryUseCase(IReadOnlyLibraryRepository readRepo, ICaching cache)
        {
            _readOnlyLibraryRepository = readRepo;
            _cache = cache;
        }

        public async Task<GetLibraryByUserIdResponse> Handle(GetLibraryByUserIdQuery request, CancellationToken cancellationToken)
        {
            string cacheKey = $"Endpoint:Library - User:{request.UserId}";

            var cachedJson = await _cache.GetAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedJson))
            {
                return JsonSerializer.Deserialize<GetLibraryByUserIdResponse>(cachedJson) ?? throw new InvalidOperationException();
            }

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

            await _cache.SetAsync(cacheKey, JsonSerializer.Serialize(response));

            return response;
        }
    }
}