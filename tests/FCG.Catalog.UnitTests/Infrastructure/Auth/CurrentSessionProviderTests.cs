using FCG.Catalog.Infrastructure.Auth.Authentication;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;

namespace FCG.Catalog.UnitTests.Infrastructure.Auth;

public class CurrentSessionProviderTests
{
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;

    public CurrentSessionProviderTests()
    {
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
    }

    private CurrentSessionProvider CreateProvider(HttpContext? context = null)
    {
        _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(context);
        return new CurrentSessionProvider(_mockHttpContextAccessor.Object);
    }

    [Fact]
    public void GetUserId_ShouldReturnNull_WhenUserIsNotAuthenticated()
    {
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
        var context = new DefaultHttpContext { User = claimsPrincipal };
        var provider = CreateProvider(context);

        var result = provider.GetUserId();

        result.Should().BeNull();
    }

    [Fact]
    public void GetUserId_ShouldReturnNull_WhenHttpContextIsNull()
    {
        var provider = CreateProvider(null);

        var result = provider.GetUserId();

        result.Should().BeNull();
    }

    [Fact]
    public void GetUserId_ShouldReturnUserId_WhenUserHasNameIdentifierClaim()
    {
        var userId = Guid.NewGuid();
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var context = new DefaultHttpContext { User = claimsPrincipal };
        var provider = CreateProvider(context);

        var result = provider.GetUserId();

        result.Should().Be(userId);
    }

    [Fact]
    public void GetUserId_ShouldReturnUserId_WhenUserHasSubClaim()
    {
        var userId = Guid.NewGuid();
        var claims = new[]
        {
            new Claim("sub", userId.ToString())
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var context = new DefaultHttpContext { User = claimsPrincipal };
        var provider = CreateProvider(context);

        var result = provider.GetUserId();

        result.Should().Be(userId);
    }

    [Fact]
    public void GetUserId_ShouldReturnNull_WhenClaimIsNotValidGuid()
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "invalid-guid")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var context = new DefaultHttpContext { User = claimsPrincipal };
        var provider = CreateProvider(context);

        var result = provider.GetUserId();

        result.Should().BeNull();
    }

    [Fact]
    public void GetUserName_ShouldReturnNull_WhenUserIsNotAuthenticated()
    {
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
        var context = new DefaultHttpContext { User = claimsPrincipal };
        var provider = CreateProvider(context);

        var result = provider.GetUserName();

        result.Should().BeNull();
    }

    [Fact]
    public void GetUserName_ShouldReturnNull_WhenHttpContextIsNull()
    {
        var provider = CreateProvider(null);

        var result = provider.GetUserName();

        result.Should().BeNull();
    }

    [Fact]
    public void GetUserName_ShouldReturnUserName_WhenUserHasNameClaim()
    {
        var userName = "testuser@example.com";
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, userName)
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var context = new DefaultHttpContext { User = claimsPrincipal };
        var provider = CreateProvider(context);

        var result = provider.GetUserName();

        result.Should().Be(userName);
    }

    [Fact]
    public void GetUserName_ShouldReturnUserName_WhenUserHasNameClaimWithLowercase()
    {
        var userName = "testuser@example.com";
        var claims = new[]
        {
            new Claim("name", userName)
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var context = new DefaultHttpContext { User = claimsPrincipal };
        var provider = CreateProvider(context);

        var result = provider.GetUserName();

        result.Should().Be(userName);
    }

    [Fact]
    public void GetCorrelationId_ShouldReturnNull_WhenCorrelationIdIsNotInItems()
    {
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
        var context = new DefaultHttpContext { User = claimsPrincipal };
        var provider = CreateProvider(context);

        var result = provider.GetCorrelationId();

        result.Should().BeNull();
    }

    [Fact]
    public void GetCorrelationId_ShouldReturnNull_WhenHttpContextIsNull()
    {
        var provider = CreateProvider(null);

        var result = provider.GetCorrelationId();

        result.Should().BeNull();
    }

    [Fact]
    public void GetCorrelationId_ShouldReturnCorrelationId_WhenCorrelationIdIsInItems()
    {
        var correlationId = Guid.NewGuid();
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
        var context = new DefaultHttpContext
        {
            User = claimsPrincipal,
            Items = { ["CorrelationId"] = correlationId.ToString() }
        };
        var provider = CreateProvider(context);

        var result = provider.GetCorrelationId();

        result.Should().Be(correlationId);
    }

    [Fact]
    public void GetCorrelationId_ShouldReturnNull_WhenCorrelationIdIsNotValidGuid()
    {
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
        var context = new DefaultHttpContext
        {
            User = claimsPrincipal,
            Items = { ["CorrelationId"] = "invalid-guid" }
        };
        var provider = CreateProvider(context);

        var result = provider.GetCorrelationId();

        result.Should().BeNull();
    }
}
