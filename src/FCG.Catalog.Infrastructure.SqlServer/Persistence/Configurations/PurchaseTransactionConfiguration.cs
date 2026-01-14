using FCG.Catalog.Domain.Catalog.Entities.Games;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FCG.Catalog.Infrastructure.SqlServer.Persistence.Configurations;

public class PurchaseTransactionConfiguration : IEntityTypeConfiguration<PurchaseTransaction>
{
    public void Configure(EntityTypeBuilder<PurchaseTransaction> builder)
    {
        builder.ToTable("PurchaseTransactions");

        builder.Property(t => t.Id)
            .HasColumnName("CorrelationId")
            .IsRequired()
            .ValueGeneratedNever();

        builder.HasKey(t => t.Id);

        builder.Ignore(t => t.CorrelationId);

        builder.Property(t => t.Status)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(t => t.Message)
            .HasMaxLength(255)
            .IsRequired(false);

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.UpdatedAt);

        builder.Property(t => t.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(t => t.Status)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(t => t.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(t => t.UserId)
            .IsRequired();

        builder.Property(t => t.GameId)
            .IsRequired();

        builder.HasIndex(t => t.UserId);
        builder.HasIndex(t => t.GameId);
        builder.HasIndex(t => t.Status);
    }
}