using FCG.Catalog.Application.UseCases.Libraries.Get;

namespace FCG.Catalog.UnitTests.UseCases.Libraries.Get;

public class GetLibraryByUserIdResponseTests
{
    [Fact]
    public void GetLibraryByUserIdResponse_ShouldHaveCorrectProperties()
    {
        // Arrange
        var libraryId = Guid.NewGuid();
        var libraryGames = new List<LibraryGameDto>
        {
            new LibraryGameDto { GameId = Guid.NewGuid(), Title = "Game 1", PurchasePrice = 49.99m },
            new LibraryGameDto { GameId = Guid.NewGuid(), Title = "Game 2", PurchasePrice = 59.99m }
        };

        // Act
        var response = new GetLibraryByUserIdResponse
        {
            LibraryId = libraryId,
            LibraryGames = libraryGames
        };

        // Assert
        Assert.Equal(libraryId, response.LibraryId);
        Assert.Equal(libraryGames, response.LibraryGames);
    }

    [Fact]
    public void GetLibraryByUserIdResponse_ShouldAllowNullLibraryGames()
    {
        // Arrange
        var libraryId = Guid.NewGuid();

        // Act
        var response = new GetLibraryByUserIdResponse
        {
            LibraryId = libraryId,
            LibraryGames = null
        };

        // Assert
        Assert.Equal(libraryId, response.LibraryId);
        Assert.Null(response.LibraryGames);
    }

    [Fact]
    public void GetLibraryByUserIdResponse_ShouldAllowEmptyLibraryGames()
    {
        // Arrange
        var libraryId = Guid.NewGuid();
        var libraryGames = new List<LibraryGameDto>();

        // Act
        var response = new GetLibraryByUserIdResponse
        {
            LibraryId = libraryId,
            LibraryGames = libraryGames
        };

        // Assert
        Assert.Equal(libraryId, response.LibraryId);
        Assert.Empty(response.LibraryGames);
    }

    [Fact]
    public void GetLibraryByUserIdResponse_ShouldSupportEquality()
    {
        // Arrange
        var libraryId = Guid.NewGuid();
        var libraryGames = new List<LibraryGameDto>
        {
            new LibraryGameDto { GameId = Guid.NewGuid(), Title = "Game 1", PurchasePrice = 49.99m }
        };
        var response1 = new GetLibraryByUserIdResponse
        {
            LibraryId = libraryId,
            LibraryGames = libraryGames
        };
        var response2 = new GetLibraryByUserIdResponse
        {
            LibraryId = libraryId,
            LibraryGames = libraryGames
        };

        // Act & Assert
        Assert.Equal(response1, response2);
    }

    [Fact]
    public void GetLibraryByUserIdResponse_ShouldNotBeEqualWhenPropertiesDiffer()
    {
        // Arrange
        var libraryId1 = Guid.NewGuid();
        var libraryId2 = Guid.NewGuid();
        var response1 = new GetLibraryByUserIdResponse
        {
            LibraryId = libraryId1,
            LibraryGames = null
        };
        var response2 = new GetLibraryByUserIdResponse
        {
            LibraryId = libraryId2,
            LibraryGames = null
        };

        // Act & Assert
        Assert.NotEqual(response1, response2);
    }

    [Fact]
    public void GetLibraryByUserIdResponse_ShouldHaveSameHashCodeForEqualInstances()
    {
        // Arrange
        var libraryId = Guid.NewGuid();
        var libraryGames = new List<LibraryGameDto>
        {
            new LibraryGameDto { GameId = Guid.NewGuid(), Title = "Game 1", PurchasePrice = 49.99m }
        };
        var response1 = new GetLibraryByUserIdResponse
        {
            LibraryId = libraryId,
            LibraryGames = libraryGames
        };
        var response2 = new GetLibraryByUserIdResponse
        {
            LibraryId = libraryId,
            LibraryGames = libraryGames
        };

        // Act
        var hash1 = response1.GetHashCode();
        var hash2 = response2.GetHashCode();

        // Assert
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void GetLibraryByUserIdResponse_ShouldHaveDifferentHashCodeForDifferentInstances()
    {
        // Arrange
        var response1 = new GetLibraryByUserIdResponse
        {
            LibraryId = Guid.NewGuid(),
            LibraryGames = new List<LibraryGameDto>()
        };
        var response2 = new GetLibraryByUserIdResponse
        {
            LibraryId = Guid.NewGuid(),
            LibraryGames = new List<LibraryGameDto>()
        };

        // Act
        var hash1 = response1.GetHashCode();
        var hash2 = response2.GetHashCode();

        // Assert
        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void GetLibraryByUserIdResponse_ToString_ShouldReturnExpectedFormat()
    {
        // Arrange
        var libraryId = Guid.NewGuid();
        var response = new GetLibraryByUserIdResponse
        {
            LibraryId = libraryId,
            LibraryGames = null
        };

        // Act
        var toStringResult = response.ToString();

        // Assert
        Assert.Contains(libraryId.ToString(), toStringResult);
        Assert.Contains("LibraryGames =", toStringResult); // Verifica formato típico de record
    }

    [Fact]
    public void GetLibraryByUserIdResponse_With_ShouldCreateCopyWithModifiedProperty()
    {
        // Arrange
        var originalId = Guid.NewGuid();
        var newId = Guid.NewGuid();
        var response = new GetLibraryByUserIdResponse
        {
            LibraryId = originalId,
            LibraryGames = new List<LibraryGameDto>()
        };

        // Act
        var modifiedResponse = response with { LibraryId = newId };

        // Assert
        Assert.Equal(originalId, response.LibraryId); // Original inalterado
        Assert.Equal(newId, modifiedResponse.LibraryId);
        Assert.Equal(response.LibraryGames, modifiedResponse.LibraryGames); // Outra propriedade permanece igual
    }

    [Fact]
    public void GetLibraryByUserIdResponse_ShouldNotBeEqualToNull()
    {
        // Arrange
        var response = new GetLibraryByUserIdResponse
        {
            LibraryId = Guid.NewGuid(),
            LibraryGames = null
        };

        // Act & Assert
        Assert.False(response.Equals(null));
        Assert.False(response == null);
        Assert.True(response != null);
    }
}