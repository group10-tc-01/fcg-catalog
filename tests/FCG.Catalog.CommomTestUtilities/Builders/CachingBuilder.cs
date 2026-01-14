using FCG.Catalog.Infrastructure.Redis.Interface;
using Moq;

namespace FCG.Catalog.CommomTestUtilities.Builders
{
    public static class CachingBuilder
    {
        private static readonly Mock<ICaching> _mock = new Mock<ICaching>();

        public static ICaching Build() => _mock.Object;

        public static void Reset() => _mock.Reset();

        public static void SetupGetAsync(string key, string? value)
        {
            _mock.Setup(cache => cache.GetAsync(key))!
                .ReturnsAsync(value);
        }

        public static void SetupSetAsync()
        {
            _mock.Setup(cache => cache.SetAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);
        }

        public static void SetupRemoveAsync()
        {
            _mock.Setup(cache => cache.RemoveAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);
        }

        public static void VerifyGetAsyncWasCalled(string key, Times times)
        {
            _mock.Verify(cache => cache.GetAsync(key), times);
        }

        public static void VerifySetAsyncWasCalled(string key, Times times)
        {
            _mock.Verify(cache => cache.SetAsync(key, It.IsAny<string>()), times);
        }

        public static void VerifySetAsyncWasNeverCalled()
        {
            _mock.Verify(cache => cache.SetAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }

        public static void VerifyRemoveAsyncWasCalled(string key, Times times)
        {
            _mock.Verify(cache => cache.RemoveAsync(key), times);
        }
    }
}