using FluentAssertions;
using System.Net;
using FCG.Catalog.IntegratedTests.Configurations;

namespace FCG.Catalog.IntegratedTests.Controllers
{
    public class HealthCheckTests : FcgCatalogFixture
    {
        public HealthCheckTests(CustomWebApplicationFactory factory) : base(factory) { }

        [Fact]
        public async Task Health_ShouldReturnOk_WhenRunningInTestEnvironmentWithoutElasticsearch()
        {
            var response = await _httpClient.GetAsync("/health");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
