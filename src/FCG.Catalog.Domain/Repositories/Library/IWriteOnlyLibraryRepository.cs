using System.Threading.Tasks;
using FCG.Catalog.Domain.Catalog.Entities.Libraries;

namespace FCG.Domain.Repositories.LibraryRepository
{
    public interface IWriteOnlyLibraryRepository
    {
        Task AddAsync(Library library);
    }
}