using FCG.Catalog.Application.UseCases.Promotion.Create;
using FCG.Catalog.Application.UseCases.Promotion.Update;
using FCG.Catalog.Application.UseCases.Promotion.Delete;
using FCG.Catalog.IntegratedTests.Configurations;
using FCG.Catalog.WebApi.Models;
using FluentAssertions;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FCG.Catalog.IntegratedTests.Controllers
{
    public class PromotionsControllerTest : FcgCatalogFixture
    {
        private const string BaseUrl = "/api/Promotion";

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter() }
        };

        public PromotionsControllerTest(CustomWebApplicationFactory factory)
            : base(factory)
        {
        }

        #region CREATE

        [Fact]
        public async Task Given_ValidInput_When_CreatePromotionIsCalled_ShouldReturnCreated()
        {
            // Arrange
            var game = Factory.CreatedGames.Last();

            var input = new CreatePromotionInput
            {
                GameId = game.Id,
                DiscountPercentage = 15,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(10)
            };

            var token = GenerateToken(Guid.NewGuid(), "Admin");

            // Act
            var result = await DoAuthenticatedPost(BaseUrl, input, token);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.Created);

            var content = await result.Content.ReadAsStringAsync();
            var response = JsonSerializer.Deserialize<ApiResponse<CreatePromotionOutput>>(content, JsonOptions);

            response.Should().NotBeNull();
            response!.Success.Should().BeTrue();
            response.Data.Should().NotBeNull();
            response.Data!.GameId.Should().Be(game.Id);
            response.Data.Discount.Should().Be(15);
        }

        [Fact]
        public async Task Given_NonExistingGame_When_CreatePromotionIsCalled_ShouldReturnNotFound()
        {
            // Arrange
            var input = new CreatePromotionInput
            {
                GameId = Guid.NewGuid(),
                DiscountPercentage = 10,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(5)
            };

            var token = GenerateToken(Guid.NewGuid(), "Admin");

            // Act
            var result = await DoAuthenticatedPost(BaseUrl, input, token);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        #endregion

        #region UPDATE

        [Fact]
        public async Task Given_ExistingPromotion_When_UpdateIsCalled_ShouldReturnOk()
        {
            // Arrange
            var promotion = Factory.CreatedPromotions.First();

            var input = new UpdatePromotionInput
            {
                GameId = promotion.GameId,
                DiscountPercentage = 30,
                StartDate = promotion.StartDate.AddDays(1),
                EndDate = promotion.EndDate.AddDays(5)
            };

            var token = GenerateToken(Guid.NewGuid(), "Admin");

            // Act
            var result = await DoAuthenticatedPut($"{BaseUrl}/{promotion.Id}", input, token);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await result.Content.ReadAsStringAsync();
            var response = JsonSerializer.Deserialize<ApiResponse<UpdatePromotionOutput>>(content, JsonOptions);

            response.Should().NotBeNull();
            response!.Success.Should().BeTrue();
            response.Data!.Id.Should().Be(promotion.Id);
            response.Data.Discount.Should().Be(30);
        }

        [Fact]
        public async Task Given_NonExistingPromotion_When_UpdateIsCalled_ShouldReturnNotFound()
        {
            // Arrange
            var input = new UpdatePromotionInput
            {
                GameId = Factory.CreatedGames.First().Id,
                DiscountPercentage = 25,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(3)
            };

            var token = GenerateToken(Guid.NewGuid(), "Admin");

            // Act
            var result = await DoAuthenticatedPut($"{BaseUrl}/{Guid.NewGuid()}", input, token);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        #endregion

        #region DELETE

        [Fact]
        public async Task Given_ExistingPromotion_When_DeleteIsCalled_ShouldReturnOk()
        {
            // Arrange
            var promotion = Factory.CreatedPromotions.Last();
            var token = GenerateToken(Guid.NewGuid(), "Admin");

            // Act
            var result = await DoAuthenticatedDelete($"{BaseUrl}/{promotion.Id}", token);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await result.Content.ReadAsStringAsync();
            var response = JsonSerializer.Deserialize<ApiResponse<DeletePromotionOutput>>(content, JsonOptions);

            response.Should().NotBeNull();
            response!.Success.Should().BeTrue();
            response.Data!.Id.Should().Be(promotion.Id);
        }

        [Fact]
        public async Task Given_NonExistingPromotion_When_DeleteIsCalled_ShouldReturnNotFound()
        {
            // Arrange
            var token = GenerateToken(Guid.NewGuid(), "Admin");

            // Act
            var result = await DoAuthenticatedDelete($"{BaseUrl}/{Guid.NewGuid()}", token);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        #endregion
    }
}