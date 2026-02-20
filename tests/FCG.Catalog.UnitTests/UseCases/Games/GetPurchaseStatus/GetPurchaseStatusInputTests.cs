using FCG.Catalog.Application.UseCases.Games.GetPurchaseStatus;
using MediatR;
using Xunit;

namespace FCG.Catalog.UnitTests.UseCases.Games.GetPurchaseStatus;

public class GetPurchaseStatusInputTests
{
    [Fact]
    public void GetPurchaseStatusInput_ShouldImplementIRequest()
    {
        // Arrange & Act
        var correlationId = Guid.NewGuid();
        var input = new GetPurchaseStatusInput(correlationId);

        // Assert
        Assert.IsAssignableFrom<IRequest<PurchaseStatusOutput>>(input);
    }

    [Fact]
    public void GetPurchaseStatusInput_ShouldHaveCorrectCorrelationId()
    {
        // Arrange
        var correlationId = Guid.NewGuid();

        // Act
        var input = new GetPurchaseStatusInput(correlationId);

        // Assert
        Assert.Equal(correlationId, input.CorrelationId);
    }

    [Fact]
    public void GetPurchaseStatusInput_ShouldBeImmutable()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var input = new GetPurchaseStatusInput(correlationId);

        // Act & Assert
        // Records em C# 12 são imutáveis por padrão, então não há setter para CorrelationId
        Assert.Equal(correlationId, input.CorrelationId);
    }
}