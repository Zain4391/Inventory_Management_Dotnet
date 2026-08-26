using Inventory_Management_dotnet.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory_Management_dotnet.Data.Configs;

public sealed class AppUserConfig: IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> b)
    {
        b.ToTable("Users");

        b.Property(u => u.Name)
        .IsRequired()
        .HasMaxLength(120);

        b.Property(u => u.Age)
        .IsRequired();

        b.Property(u => u.Gender)
        .HasConversion<string>()
        .IsRequired();

        b.HasIndex(u => u.NormalizedUserName)
        .HasDatabaseName("User_NormalizedUserName_Index");

        b.HasIndex(u => u.NormalizedEmail)
        .HasDatabaseName("User_NormalizedEmail_Index");

    }
}