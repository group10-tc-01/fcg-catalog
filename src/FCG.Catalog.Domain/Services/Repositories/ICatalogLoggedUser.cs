namespace FCG.Catalog.Domain.Services.Repositories
{
    public interface ICatalogLoggedUser
    {
        Task<LoggedUserInfo?> GetLoggedUserAsync();
    }

}
