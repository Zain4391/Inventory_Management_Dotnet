namespace Inventory_Management_dotnet.Models.Entities;

public class ProductImage
{
    public Guid Id {get; set;}

    public required string ImageUrl {get; set;}

    // FK, links to Product Table
    public Guid ProductId {get; set;}

    // Reference Property, Makes things easy
    public Product Product {get; set;} = null!;
}