using System.Threading;
using System.Threading.Tasks;
using FCG.Catalog.Domain;

namespace FCG.Catalog.Infrastructure.SqlServer
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FcgCatalogDbContext _context;

        public UnitOfWork(FcgCatalogDbContext context)
        {
            _context = context;
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}
