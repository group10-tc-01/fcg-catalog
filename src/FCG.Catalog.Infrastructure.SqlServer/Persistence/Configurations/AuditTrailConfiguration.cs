using FCG.Catalog.Domain.Catalog.Entities.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FCG.Catalog.Infrastructure.SqlServer.Persistence.Configurations;

public class AuditTrailConfiguration : IEntityTypeConfiguration<AuditTrail>
{
    public void Configure(
       EntityTypeBuilder<AuditTrail> builder)
    {
        builder.ToTable("AuditTrails");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.EntityName)
            .IsRequired()
            .HasMaxLength(100) 
            .HasColumnType("varchar(100)"); 

        builder.Property(x => x.Action)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnType("varchar(50)");

        builder.Property(x => x.EntityPrimaryKey)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnType("varchar(255)");

        builder.Property(x => x.OldValue)
            .IsRequired(false);

        builder.Property(x => x.NewValue)
            .IsRequired(false);

        builder.Property(x => x.OccurredAt)
            .IsRequired();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.UserName)
            .IsRequired(false)
            .HasMaxLength(200)
            .HasColumnType("varchar(200)");

        builder.Property(x => x.CorrelationId)
            .IsRequired(false)
            .HasMaxLength(100)
            .HasColumnType("varchar(100)");

        builder.HasIndex(x => new { x.EntityName, x.EntityPrimaryKey }); 
        
        builder.HasIndex(x => x.UserId);
        
        builder.HasIndex(x => x.OccurredAt);
        
        builder.HasIndex(x => x.CorrelationId);
    }
}