using FCG.Catalog.Domain.Catalog.Entity.Promotions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FCG.Catalog.Infrastructure.SqlServer.Persistence.Configurations
{
    public class PromotionConfiguration : BaseConfiguration<Promotion>
    {
        public override void Configure(EntityTypeBuilder<Promotion> builder)
        {
            base.Configure(builder);

            builder.ToTable("Promotions", t =>
            {
                t.HasCheckConstraint("CK_Promotion_Discount", "DiscountPercentage > 0 AND DiscountPercentage <= 100");
                t.HasCheckConstraint("CK_Promotion_Dates", "EndDate > StartDate");
            });

            builder.OwnsOne(p => p.DiscountPercentage, discountBuilder =>
            {
                discountBuilder.Property(d => d.Value)
                    .HasColumnName("DiscountPercentage")
                    .HasColumnType("decimal(5,2)")
                    .IsRequired();
            });

            builder.Property(e => e.StartDate)
                .HasColumnType("datetime2")
                .IsRequired();

            builder.Property(e => e.EndDate)
                .HasColumnType("datetime2")
                .IsRequired();

            builder.Property(e => e.GameId)
                .IsRequired();

            builder.HasOne(e => e.Game)
                .WithMany(g => g.Promotions)
                .HasForeignKey(e => e.GameId)
                .OnDelete(DeleteBehavior.Cascade); 

            builder.HasIndex(e => e.GameId)
                .HasDatabaseName("IX_Promotions_GameId");

            builder.HasIndex(e => new { e.StartDate, e.EndDate })
                .HasDatabaseName("IX_Promotions_Dates");
        }
    }
}
