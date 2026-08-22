using Inventory_Management_dotnet.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory_Management_dotnet.Data.Configs;

public sealed class OrderConfig : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> b)
    {
        b.ToTable("Orders");

        b.HasKey(o => o.Id);

        b.Property(o => o.Total_items)
        .IsRequired();

        b.Property(o => o.Total_price)
        .IsRequired()
        .HasPrecision(10, 2);

        b.Property(o => o.Delivery_address)
        .IsRequired()
        .HasMaxLength(100);

        b.Property(o => o.Delivery_date)
        .IsRequired();

        b.Property(o => o.Order_status)
        .HasConversion<string>()
        .IsRequired();

        b.HasOne(o => o.User)
        .WithMany(u => u.Orders)
        .HasForeignKey(o => o.UserId)
        .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        b.HasIndex(o => o.Delivery_address)
        .HasDatabaseName("Order_Address_Index");

    }
}