using FCG.Catalog.Application.UseCases.Games.Get;
using FCG.Catalog.Application.UseCases.Games.Register;
using FCG.Catalog.Application.UseCases.Games.Update;
using FCG.Catalog.Domain.Enum;
using FCG.Catalog.Domain.Models;
using FCG.Catalog.IntegratedTests.Configurations;
using FCG.Catalog.WebApi.Models;
using FluentAssertions;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FCG.Catalog.IntegratedTests.Controllers
{
    public class GamesControllerTest : FcgCatalogFixture
    {
        private const string BaseUrl = "/api/v1/games";

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter() }
        };

        public GamesControllerTest(CustomWebApplicationFactory factory) : base(factory) { }

        #region GET

        [Fact]
        public async Task Given_ValidRequest_When_GetGamesIsCalled_ShouldReturnOk()
        {
            var url = $"{BaseUrl}?pageNumber=1&pageSize=10";
            var token = GenerateToken(Guid.NewGuid(), "User");

            var result = await DoAuthenticatedGet(url, token);

            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        #endregion

        #region REGISTER

        [Fact]
        public async Task Given_ValidRequest_When_RegisterGameIsCalled_ShouldReturnCreated()
        {
            var input = new RegisterGameInput
            {
                Name = "Integration Test Game",
                Description = "Integration test description",
                Price = 49.99m,
                Category = GameCategory.Adventure
            };

            var token = GenerateToken(Guid.NewGuid(), "Admin");

            var result = await DoAuthenticatedPost(BaseUrl, input, token);
            var content = await result.Content.ReadAsStringAsync();

            var response = JsonSerializer.Deserialize<ApiResponse<RegisterGameOutput>>(content, JsonOptions);

            result.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Should().NotBeNull();
            response!.Data.Id.Should().NotBeEmpty();
            response.Data.Name.Should().Be(input.Name);
        }

        [Fact]
        public async Task Given_DuplicateName_When_RegisterGameIsCalled_ShouldReturnConflict()
        {
            var input = new RegisterGameInput
            {
                Name = "Duplicate Game",
                Description = "Test description",
                Price = 39.99m,
                Category = GameCategory.RPG
            };

            var token = GenerateToken(Guid.NewGuid(), "Admin");

            var firstResponse = await DoAuthenticatedPost(BaseUrl, input, token);
            firstResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var secondResponse = await DoAuthenticatedPost(BaseUrl, input, token);

            secondResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        #endregion

        #region UPDATE

        [Fact]
        public async Task Given_ValidRequest_When_UpdateGameIsCalled_ShouldReturnOk()
        {
            var createInput = new RegisterGameInput
            {
                Name = "Game To Update",
                Description = "Old Description",
                Price = 29.99m,
                Category = GameCategory.Action
            };

            var adminToken = GenerateToken(Guid.NewGuid(), "Admin");

            var createResponse = await DoAuthenticatedPost(BaseUrl, createInput, adminToken);
            var createContent = await createResponse.Content.ReadAsStringAsync();

            var createdGame = JsonSerializer
                .Deserialize<ApiResponse<RegisterGameOutput>>(createContent, JsonOptions)!;

            var updateInput = new UpdateGameInput
            {
                Title = "Updated Title",
                Description = "Updated Description",
                Price = 59.99m,
                Category = GameCategory.RPG
            };

            var updateResponse = await DoAuthenticatedPut(
                $"{BaseUrl}/{createdGame.Data.Id}",
                updateInput,
                adminToken);

            updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        #endregion

        #region GET BY ID

        [Fact]
        public async Task Given_ValidId_When_GetGameByIdIsCalled_ShouldReturnOk()
        {
            var input = new RegisterGameInput
            {
                Name = "Game GetById",
                Description = "Test",
                Price = 19.99m,
                Category = GameCategory.Sports
            };

            var adminToken = GenerateToken(Guid.NewGuid(), "Admin");

            var createResponse = await DoAuthenticatedPost(BaseUrl, input, adminToken);
            var content = await createResponse.Content.ReadAsStringAsync();

            var createdGame = JsonSerializer
                .Deserialize<ApiResponse<RegisterGameOutput>>(content, JsonOptions)!;

            var userToken = GenerateToken(Guid.NewGuid(), "User");

            var result = await DoAuthenticatedGet(
                $"{BaseUrl}/{createdGame.Data.Id}",
                userToken);

            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Given_InvalidId_When_GetGameByIdIsCalled_ShouldReturnNotFound()
        {
            var token = GenerateToken(Guid.NewGuid(), "User");
            var invalidId = Guid.NewGuid();

            var result = await DoAuthenticatedGet($"{BaseUrl}/{invalidId}", token);

            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        #endregion

        #region DELETE

        [Fact]
        public async Task Given_ValidRequest_When_DeleteGameIsCalled_ShouldReturnNoContent()
        {
            var input = new RegisterGameInput
            {
                Name = "Game To Delete",
                Description = "Test",
                Price = 19.99m,
                Category = GameCategory.Sports
            };

            var adminToken = GenerateToken(Guid.NewGuid(), "Admin");

            var createResponse = await DoAuthenticatedPost(BaseUrl, input, adminToken);
            var content = await createResponse.Content.ReadAsStringAsync();

            var createdGame = JsonSerializer
                .Deserialize<ApiResponse<RegisterGameOutput>>(content, JsonOptions)!;

            var deleteResponse = await DoAuthenticatedDelete(
                $"{BaseUrl}/{createdGame.Data.Id}",
                adminToken);

            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Given_InvalidId_When_DeleteGameIsCalled_ShouldReturnNotFound()
        {
            var token = GenerateToken(Guid.NewGuid(), "Admin");
            var invalidId = Guid.NewGuid();

            var result = await DoAuthenticatedDelete($"{BaseUrl}/{invalidId}", token);

            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        #endregion
    }
}
