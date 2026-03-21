namespace FCG.Catalog.Domain.Abstractions
{
    public interface ICurrentSessionProvider
    {
        Guid? GetUserId();
    }
}
