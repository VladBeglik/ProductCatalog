using Catalog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder
            .HasKey(_ => _.Id);

        builder
            .HasOne(_ => _.Category)
            .WithMany(_ => _.Products)
            .HasForeignKey(_ => _.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Property(p => p.Price)
            .HasColumnType("decimal(18, 2)");
    }
}