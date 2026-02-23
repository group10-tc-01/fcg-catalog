using FCG.Catalog.Domain.Catalog.Entities.Games;
using FluentAssertions;

namespace FCG.Catalog.UnitTests.Domain.Catalog
{
    public class PurchaseTransactionTests
    {
        [Theory]
        [InlineData(59.99)]
        [InlineData(0)]
        [InlineData(-10)]
        public void Given_Amount_When_ConstructingPurchaseTransaction_Then_ShouldSetAmountCorrectly(decimal amount)
        {
            // Arrange
            var correlationId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var gameId = Guid.NewGuid();

            // Act
            var transaction = new PurchaseTransaction(correlationId, userId, gameId, amount);

            // Assert
            transaction.Amount.Should().Be(amount);
            transaction.Status.Should().Be("Pending");
            transaction.Message.Should().BeNull();
        }

        [Fact]
        public void Given_ValidParameters_When_ConstructingPurchaseTransaction_Then_ShouldSetAllPropertiesCorrectly()
        {
            // Arrange
            var correlationId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var gameId = Guid.NewGuid();
            var amount = 59.99m;

            // Act
            var transaction = new PurchaseTransaction(correlationId, userId, gameId, amount);

            // Assert
            transaction.CorrelationId.Should().Be(correlationId);
            transaction.Id.Should().Be(correlationId);
            transaction.UserId.Should().Be(userId);
            transaction.GameId.Should().Be(gameId);
        }

        [Theory]
        [InlineData("Completed", "Purchase successful")]
        [InlineData("Failed", null)]
        [InlineData("Processing", "In progress")]
        public void Given_StatusAndMessage_When_UpdateStatus_Then_ShouldUpdateCorrectly(string status, string? message)
        {
            // Arrange
            var transaction = new PurchaseTransaction(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 49.99m);
            var initialUpdatedAt = transaction.UpdatedAt;

            // Act
            transaction.UpdateStatus(status, message);

            // Assert
            transaction.Status.Should().Be(status);
            transaction.Message.Should().Be(message);
            transaction.UpdatedAt.Should().BeAfter(initialUpdatedAt);
        }

        [Fact]
        public void Given_MultipleUpdates_When_UpdateStatus_Then_ShouldUpdateEachTimeWithNewTimestamp()
        {
            // Arrange
            var transaction = new PurchaseTransaction(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 29.99m);

            // Act
            transaction.UpdateStatus("Processing");
            var firstUpdate = transaction.UpdatedAt;
            System.Threading.Thread.Sleep(1); // Ensure timestamp difference
            transaction.UpdateStatus("Completed", "Done");

            // Assert
            transaction.Status.Should().Be("Completed");
            transaction.Message.Should().Be("Done");
            transaction.UpdatedAt.Should().BeAfter(firstUpdate);
        }

        [Fact]
        public void Given_CorrelationId_When_ConstructingPurchaseTransaction_Then_IdShouldMatchCorrelationId()
        {
            // Arrange
            var correlationId = Guid.NewGuid();

            // Act
            var transaction = new PurchaseTransaction(correlationId, Guid.NewGuid(), Guid.NewGuid(), 19.99m);

            // Assert
            transaction.Id.Should().Be(correlationId);
            transaction.CorrelationId.Should().Be(correlationId);
        }
    }
}