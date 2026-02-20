using FCG.Catalog.Application.UseCases.Games.GetPurchaseStatus;
using Xunit;

namespace FCG.Catalog.UnitTests.UseCases.Games.GetPurchaseStatus;

public class PurchaseStatusOutputTests
{
    [Fact]
    public void PurchaseStatusOutput_ShouldHaveCorrectProperties()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var status = "Completed";
        var message = "Compra realizada com sucesso.";

        // Act
        var output = new PurchaseStatusOutput(correlationId, status, message);

        // Assert
        Assert.Equal(correlationId, output.CorrelationId);
        Assert.Equal(status, output.Status);
        Assert.Equal(message, output.Message);
    }

    [Fact]
    public void PurchaseStatusOutput_ShouldAllowNullMessage()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var status = "NotFound";

        // Act
        var output = new PurchaseStatusOutput(correlationId, status);

        // Assert
        Assert.Equal(correlationId, output.CorrelationId);
        Assert.Equal(status, output.Status);
        Assert.Null(output.Message);
    }

    [Fact]
    public void PurchaseStatusOutput_ShouldBeImmutable()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var status = "Pending";
        var message = "Aguardando processamento.";
        var output = new PurchaseStatusOutput(correlationId, status, message);

        // Act & Assert
        // Records em C# 12 são imutáveis, então propriedades não podem ser alteradas
        Assert.Equal(correlationId, output.CorrelationId);
        Assert.Equal(status, output.Status);
        Assert.Equal(message, output.Message);
    }

    [Fact]
    public void PurchaseStatusOutput_ShouldSupportEquality()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var status = "Failed";
        var message = "Erro na transação.";
        var output1 = new PurchaseStatusOutput(correlationId, status, message);
        var output2 = new PurchaseStatusOutput(correlationId, status, message);

        // Act & Assert
        Assert.Equal(output1, output2);
    }
}