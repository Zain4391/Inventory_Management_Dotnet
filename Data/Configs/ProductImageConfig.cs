using Inventory_Management_dotnet.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory_Management_dotnet.Configs;

public sealed class ProductImageConfig : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> b)
    {
        // Table Name
        b.ToTable("Product_Images");

        // PK + Alt Key
        b.HasKey(pi => pi.Id);

        // Properties
        b.Property(pi => pi.ImageUrl)
        .IsRequired();

        // Relationships
        b.HasOne(pi => pi.Product)
        .WithMany(p => p.Images)
        .HasForeignKey(pi => pi.ProductId)
        .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        b.HasIndex(pi => pi.ImageUrl).IsUnique();
    }
}