namespace FCG.Catalog.Application.Services
{
    public interface IGameSearchReindexService
    {
        Task ReindexAsync(CancellationToken cancellationToken = default);
    }
}
