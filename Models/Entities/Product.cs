namespace Inventory_Management_dotnet.Models.Entities;

public enum CATEGORY
{
    Electronics,
    Clothing,
    Food,
    VideoGames,
    Others
}

public class Product
{
    public Guid Id {get; set;}

    public required string Name {get; set;}

    public string? Description {get; set;}

    public CATEGORY Category {get; set;}

    public int Quantity {get; set;}

    public decimal Price {get; set;}

    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;

    // Collection of Product Images
    public ICollection<ProductImage> Images {get; set;} = [];    
}