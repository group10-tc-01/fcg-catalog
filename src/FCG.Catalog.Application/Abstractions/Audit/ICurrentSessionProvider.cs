namespace FCG.Catalog.Application.Abstractions.Audit
{
    public interface ICurrentSessionProvider
    {
        Guid? GetUserId();
    }
}
