using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;
using FCG.Catalog.Domain.Catalog.Entities.Games;

namespace FCG.Catalog.Infrastructure.SqlServer.Persistence.Configurations
{
    [ExcludeFromCodeCoverage]
    public class GameConfiguration : BaseConfiguration<Game>
    {
        public override void Configure(EntityTypeBuilder<Game> builder)
        {
            base.Configure(builder);

            builder.ToTable("Games", t => t.HasCheckConstraint("CK_Game_Price", "Price > 0"));

            builder.Property(g => g.Description)
                .HasMaxLength(2000) 
                .IsRequired(false);

            builder.OwnsOne(g => g.Title, titleBuilder =>
            {
                titleBuilder.Property(n => n.Value)
                    .HasColumnName("Title")
                    .HasMaxLength(200)
                    .IsRequired();
            });

            builder.OwnsOne(g => g.Price, priceBuilder =>
            {
                priceBuilder.Property(p => p.Value)
                    .HasColumnName("Price")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();
            });

            builder.Property(g => g.Category)
                .HasConversion<string>() 
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(g => g.IsActive)
                .HasDefaultValue(true)
                .IsRequired();

            builder.HasIndex(g => g.Category)
                .HasDatabaseName("IX_Games_Category");

            builder.HasIndex(g => g.IsActive)
                .HasDatabaseName("IX_Games_IsActive");
        }
    }
}
