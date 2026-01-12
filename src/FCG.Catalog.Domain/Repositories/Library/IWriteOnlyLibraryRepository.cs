namespace FCG.Catalog.Domain.Repositories.Library
{
    public interface IWriteOnlyLibraryRepository
    {
        Task AddAsync(FCG.Catalog.Domain.Catalog.Entities.Libraries.Library library);
    }
}