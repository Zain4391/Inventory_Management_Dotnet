using Inventory_Management_dotnet.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory_Management_dotnet.Data.Configs;

public sealed class OrderItemConfig : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> b)
    {
        // Table Name
        b.ToTable("Order_Items");

        // PK + Alt Key
        b.HasKey(oi => oi.Id);

        // Properties
        b.Property(oi => oi.Quantity)
        .IsRequired();

        b.Property(oi => oi.UnitPrice)
        .HasPrecision(10, 2)
        .IsRequired();

        // Relationships
        // Line items are child data of the order, they go with it
        b.HasOne(oi => oi.Order)
        .WithMany(o => o.Items)
        .HasForeignKey(oi => oi.OrderId)
        .OnDelete(DeleteBehavior.Cascade);

        // Order history must survive: a product that was ordered cannot be hard-deleted
        b.HasOne(oi => oi.Product)
        .WithMany()
        .HasForeignKey(oi => oi.ProductId)
        .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        // One line per product per order, also serves lookups by OrderId
        b.HasIndex(oi => new { oi.OrderId, oi.ProductId })
        .IsUnique()
        .HasDatabaseName("OrderItem_Order_Product_Index");

        b.HasIndex(oi => oi.ProductId)
        .HasDatabaseName("OrderItem_ProductId_Index");
    }
}
