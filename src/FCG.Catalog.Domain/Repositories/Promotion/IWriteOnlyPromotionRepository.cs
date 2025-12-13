using System.Threading;
using System.Threading.Tasks;

namespace FCG.Catalog.Domain.Repositories.Promotion
{
    public interface IWriteOnlyPromotionRepository
    {
        Task AddAsync(Catalog.Entity.Promotions.Promotion promotion, CancellationToken cancellationToken = default);
        Task UpdateAsync(Catalog.Entity.Promotions.Promotion promotion, CancellationToken cancellationToken = default);
        Task DeleteAsync(Catalog.Entity.Promotions.Promotion promotion, CancellationToken cancellationToken = default);
    }
}
