using FCG.Catalog.Application.UseCases.Libraries.Get;
using FCG.Catalog.IntegratedTests.Configurations;
using FCG.Catalog.WebApi.Models;
using FluentAssertions;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FCG.Catalog.IntegratedTests.Controllers
{
    public class LibrariesControllerTest : FcgCatalogFixture
    {
        private const string BaseUrl = "/api/Library";

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter() }
        };

        public LibrariesControllerTest(CustomWebApplicationFactory factory)
            : base(factory)
        {
        }

        [Fact]
        public async Task Given_AuthenticatedUser_WithExistingLibrary_When_GetLibraryIsCalled_ShouldReturnOkAndLibrary()
        {
            // Arrange
            var existingLibrary = Factory.CreatedLibraries.First();
            var token = GenerateToken(existingLibrary.UserId, "User");

            // Act
            var result = await DoAuthenticatedGet(BaseUrl, token);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await result.Content.ReadAsStringAsync();
            var response = JsonSerializer.Deserialize<
                ApiResponse<GetLibraryByUserIdResponse>>(content, JsonOptions);

            response.Should().NotBeNull();
            response!.Success.Should().BeTrue();
            response.Data.Should().NotBeNull();
            response.Data!.LibraryId.Should().Be(existingLibrary.Id);
        }

        [Fact]
        public async Task Given_AuthenticatedUser_WithoutLibrary_When_GetLibraryIsCalled_ShouldReturnOkWithEmptyLibrary()
        {
            // Arrange
            var userIdWithoutLibrary = Guid.NewGuid();
            var token = GenerateToken(userIdWithoutLibrary, "User");

            // Act
            var result = await DoAuthenticatedGet(BaseUrl, token);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await result.Content.ReadAsStringAsync();
            var response = JsonSerializer.Deserialize<
                ApiResponse<GetLibraryByUserIdResponse>>(content, JsonOptions);

            response.Should().NotBeNull();
            response!.Success.Should().BeTrue();
            response.Data.Should().NotBeNull();
            response.Data!.LibraryId.Should().Be(Guid.Empty);
            response.Data.LibraryGames.Should().BeEmpty();
        }

        [Fact]
        public async Task Given_RequestWithoutToken_When_GetLibraryIsCalled_ShouldReturnUnauthorized()
        {
            // Act
            var result = await _httpClient.GetAsync(BaseUrl);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Given_TokenWithEmptyUserId_When_GetLibraryIsCalled_ShouldReturnUnauthorized()
        {
            // Arrange
            var token = GenerateToken(Guid.Empty, "User");

            // Act
            var result = await DoAuthenticatedGet(BaseUrl, token);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            var content = await result.Content.ReadAsStringAsync();
            content.Should().Contain("User not authenticated");
        }
    }
}
