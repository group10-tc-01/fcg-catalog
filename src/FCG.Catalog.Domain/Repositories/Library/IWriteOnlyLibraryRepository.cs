using FCG.Catalog.Domain.Catalog.Entity.Libraries;

namespace FCG.Domain.Repositories.LibraryRepository
{
    public interface IWriteOnlyLibraryRepository
    {
        Task AddAsync(Library library);
    }
}