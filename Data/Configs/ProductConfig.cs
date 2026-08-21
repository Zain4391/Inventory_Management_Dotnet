using Inventory_Management_dotnet.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory_Management_dotnet.Configs;

public sealed class ProductConfig: IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> b)
    {
        // Table name
        b.ToTable("Products");

        // PK + Alt Key
        b.HasKey(p => p.Id);
        b.HasAlternateKey(e => e.Name).HasName("AK_Product_Name");

        // Property Rules
        b.Property(p => p.Name)
        .IsRequired()
        .HasMaxLength(120);

        b.Property(p => p.Description)
        .HasMaxLength(500);

        b.Property(p => p.Category)
        .HasConversion<string>() // DB will store enums as strings
        .IsRequired();

        b.Property(p => p.Quantity)
        .IsRequired();

        b.Property(p => p.Price)
        .HasPrecision(10, 2)
        .IsRequired();

        // Indexes
        b.HasIndex(p => p.Name);
    }
}