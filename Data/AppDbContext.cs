using Inventory_Management_dotnet.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Management_dotnet.Data;

/*
    IdentityRole<Guid> Means Roles table will have ID as Guid
    Third "Guid" means the PKs used by AppUser and related Entites are Guids

    This is being done so that an established pattern is carried forward.
*/
public class AppDbContext(DbContextOptions dbContextOptions) : 
    IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>(dbContextOptions)
{
    // DB Sets = Tables in the Data Base
    public DbSet<Product> Products {get; set;}
    public DbSet<ProductImage> ProductImages {get; set;}

    /*
        Apply Fluent API conventions
        Instead of having one big function, let .NET pick the conventions
    */
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // for IdentityDB to set up it's entities
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}