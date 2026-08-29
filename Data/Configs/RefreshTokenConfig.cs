using Inventory_Management_dotnet.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory_Management_dotnet.Data.Configs;

public sealed class RefreshTokenConfig : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> b)
    {
        b.ToTable("Refresh_Tokens");

        b.HasKey(rt => rt.Id);

        b.Property(rt => rt.TokenHash)
        .IsRequired();

        b.HasOne(rt => rt.User)
        .WithMany(u => u.RefreshTokens)
        .HasForeignKey(rt => rt.UserId)
        .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(rt => rt.TokenHash)
        .IsUnique();
    }
}