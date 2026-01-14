using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace FCG.Catalog.IntegratedTests.Configurations
{
    [ExcludeFromCodeCoverage]
    public class FcgCatalogFixture : IClassFixture<CustomWebApplicationFactory>
    {
        protected readonly HttpClient _httpClient;
        protected readonly CustomWebApplicationFactory Factory;
        private readonly IConfiguration _configuration;

        public FcgCatalogFixture(CustomWebApplicationFactory factory)
        {
            Factory = factory;
            _httpClient = factory.CreateClient();
            _configuration = factory.Services.GetRequiredService<IConfiguration>();
        }

        protected async Task<HttpResponseMessage> DoPost<T>(string url, T content)
        {
            var json = JsonSerializer.Serialize(content);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            return await _httpClient.PostAsync(url, stringContent);
        }

        protected async Task<HttpResponseMessage> DoAuthenticatedPost<T>(string url, T content, string token)
        {
            var json = JsonSerializer.Serialize(content);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return await _httpClient.PostAsync(url, stringContent);
        }

        protected async Task<HttpResponseMessage> DoPut<T>(string url, T content)
        {
            var json = JsonSerializer.Serialize(content);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            return await _httpClient.PutAsync(url, stringContent);
        }

        protected async Task<HttpResponseMessage> DoAuthenticatedPut<T>(string url, T content, string token)
        {
            var json = JsonSerializer.Serialize(content);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return await _httpClient.PutAsync(url, stringContent);
        }

        protected async Task<HttpResponseMessage> DoAuthenticatedGet(string url, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return await _httpClient.GetAsync(url);
        }

        protected async Task<HttpResponseMessage> DoAuthenticatedDelete(string url, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return await _httpClient.DeleteAsync(url);
        }

        protected string GenerateToken(Guid userId, string role)
        {
            var secretKey = _configuration["JwtSettings:SecretKey"];
            var issuer = _configuration["JwtSettings:Issuer"];
            var audience = _configuration["JwtSettings:Audience"];

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}