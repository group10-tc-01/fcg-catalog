using FCG.Catalog.Domain.Repositories.Library;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace FCG.Catalog.Application.UseCases.Libraries.Get
{
    public class GetLibraryUseCase : IRequestHandler<GetLibraryByUserIdQuery, GetLibraryByUserIdResponse>
    {
        private readonly IReadOnlyLibraryRepository _readOnlyLibraryRepository;
        private readonly IDistributedCache _cache;

        public GetLibraryUseCase(IReadOnlyLibraryRepository readRepo, IDistributedCache cache)
        {
            _readOnlyLibraryRepository = readRepo;
            _cache = cache;
        }

        public async Task<GetLibraryByUserIdResponse> Handle(GetLibraryByUserIdQuery request, CancellationToken cancellationToken)
        {
            string cacheKey = $"library:{request.UserId}";
            var cachedData = await _cache.GetStringAsync(cacheKey, cancellationToken);

            if (!string.IsNullOrEmpty(cachedData))
            {
                return JsonSerializer.Deserialize<GetLibraryByUserIdResponse>(cachedData);
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

            var gamesDto = library.LibraryGames?
                .Select(lg => new LibraryGameDto
                {
                    GameId = lg.GameId,
                    Title = lg.Game.Title.Value,
                    Description = lg.Game.Description,
                    PurchasePrice = lg.PurchasePrice.Value, 
                    PurchaseDate = lg.PurchaseDate
                })
                .OrderByDescending(g => g.PurchaseDate) 
                .ToList();

            var response =  new GetLibraryByUserIdResponse
            {
                LibraryId = library.Id,
                LibraryGames = gamesDto ?? new List<LibraryGameDto>()
            };
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(120)
            };
            await _cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(response),
                cacheOptions,
                cancellationToken);

            return response;
        }
    }
    }