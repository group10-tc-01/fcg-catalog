using FCG.Catalog.Domain.Catalog.Entity.Libraries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace FCG.Catalog.Infrastructure.SqlServer.Persistence.Configurations
{
    [ExcludeFromCodeCoverage]
    public class LibraryConfiguration : BaseConfiguration<Library>
    {
        public override void Configure(EntityTypeBuilder<Library> builder)
        {
            base.Configure(builder);

            builder.ToTable("Libraries");

            builder.Property(l => l.UserId)
                .IsRequired();

            builder.HasIndex(l => l.UserId)
                .IsUnique()
                .HasDatabaseName("IX_Libraries_UserId");

            builder.HasMany(l => l.LibraryGames)
                .WithOne()
                .HasForeignKey("LibraryId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(l => l.LibraryGames)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}