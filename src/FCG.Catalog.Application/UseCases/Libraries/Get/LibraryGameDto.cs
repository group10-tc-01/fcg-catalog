namespace FCG.Catalog.Application.UseCases.Libraries.Get
{
    public class LibraryGameDto
    {
        public Guid GameId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal PurchasePrice { get; set; }
        public DateTimeOffset PurchaseDate { get; set; }
    }
}
