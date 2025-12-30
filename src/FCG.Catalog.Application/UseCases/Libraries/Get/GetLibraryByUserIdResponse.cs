using FCG.Catalog.Domain.Catalog.Entities.LibraryGames;

namespace FCG.Catalog.Application.UseCases.Libraries.Get
{
    public record GetLibraryByUserIdResponse 
    {
        public Guid LibraryId { get; set; }
        public List<LibraryGameDto>? LibraryGames { get; set; }
    }
}
