using FCG.Catalog.Domain.Catalog.Entities.LibraryGames;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FCG.Catalog.Infrastructure.SqlServer.Persistence.Configurations
{
    public class LibraryGameConfiguration : BaseConfiguration<LibraryGame>
    {
        public override void Configure(EntityTypeBuilder<LibraryGame> builder)
        {
            base.Configure(builder);

            builder.ToTable("LibraryGames", t =>
            {
                t.HasCheckConstraint("CK_LibraryGame_Price", "PurchasePrice >= 0");
            });

            builder.OwnsOne(lg => lg.PurchasePrice, priceBuilder =>
            {
                priceBuilder.Property(p => p.Value)
                    .HasColumnName("PurchasePrice")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();
            });

            builder.Property(lg => lg.PurchaseDate)
                .HasColumnName("PurchasedAt")
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETUTCDATE()")
                .IsRequired();

            builder.HasOne(lg => lg.Library)
                .WithMany(l => l.LibraryGames)
                .HasForeignKey(lg => lg.LibraryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(lg => lg.Game)
                .WithMany()
                .HasForeignKey(lg => lg.GameId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(lg => lg.GameId)
                .HasDatabaseName("IX_LibraryGames_GameId");

            builder.HasIndex(lg => new { lg.LibraryId, lg.GameId })
                .IsUnique()
                .HasDatabaseName("UQ_LibraryGames_LibraryId_GameId");
        }
    }
}