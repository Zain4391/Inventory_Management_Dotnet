using Inventory_Management_dotnet.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory_Management_dotnet.Data.Configs;

public sealed class UserConfig: IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> b)
    {
        b.ToTable("Users");

        b.HasKey(u => u.Id);
        b.HasAlternateKey(u => u.Email).HasName("AK_User_Email");

        b.Property(u => u.Name)
        .IsRequired()
        .HasMaxLength(120);

        b.Property(u => u.Email)
        .IsRequired();

        b.Property(u => u.Age)
        .IsRequired();

        b.Property(u => u.Role)
        .HasConversion<string>()
        .HasDefaultValue(ROLES.staff);

        b.Property(u => u.Gender)
        .HasConversion<string>()
        .IsRequired();

        b.Property(u => u.PasswordHash)
        .IsRequired();

        b.HasIndex(u => u.Email)
        .HasDatabaseName("User_Email_Index");

    }
}