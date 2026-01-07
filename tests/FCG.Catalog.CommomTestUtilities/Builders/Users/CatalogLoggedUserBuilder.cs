using FCG.Catalog.Domain.Services;
using FCG.Catalog.Domain.Services.Repositories;
using Moq;

namespace FCG.Catalog.CommomTestUtilities.Builders.Users
{
    public static class CatalogLoggedUserBuilder
    {
        private static readonly Mock<ICatalogLoggedUser> _mock = new Mock<ICatalogLoggedUser>();

        public static ICatalogLoggedUser Build() => _mock.Object;

        public static void Reset() => _mock.Reset();

        public static void SetupGetLoggedUserAsync(LoggedUserInfo? loggedUser)
        {
            _mock.Setup(service => service.GetLoggedUserAsync())
                .ReturnsAsync(loggedUser);
        }

        public static void SetupGetLoggedUserAsyncThrows(Exception exception)
        {
            _mock.Setup(service => service.GetLoggedUserAsync())
                .ThrowsAsync(exception);
        }

        public static void VerifyGetLoggedUserAsyncWasCalled(Times times)
        {
            _mock.Verify(service => service.GetLoggedUserAsync(), times);
        }
    }
}