using System.Threading.Tasks;
using FCG.Catalog.Domain.Catalog.Entities.Libraries;

namespace FCG.Catalog.Domain.Repositories.Library
{
    public interface IWriteOnlyLibraryRepository
    {
        Task AddAsync(Catalog.Entities.Libraries.Library library);
    }
}