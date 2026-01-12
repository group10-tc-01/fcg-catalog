using FCG.Catalog.Domain.Catalog.Entities.Libraries;

namespace FCG.Catalog.Domain.Repositories.Library
{
    public interface IWriteOnlyLibraryRepository
    {
        Task AddAsync(Library library);
    }
}