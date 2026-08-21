using Inventory_Management_dotnet.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Management_dotnet.Data;

public class AppDbContext(DbContextOptions dbContextOptions) : DbContext(dbContextOptions)
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
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}