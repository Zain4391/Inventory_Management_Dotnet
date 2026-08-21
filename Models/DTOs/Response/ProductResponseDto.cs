using Inventory_Management_dotnet.Models.Entities;

namespace Inventory_Management_dotnet.Models.DTOs.Response;

public class ProductResponseDto
{
    public Guid Id {get; set;}
    public required string Name {get; set;}
    public string? Description {get; set;}
    public CATEGORY Category {get; set;}
    public int Quantity {get; set;}
    public decimal Price {get; set;}

    // list of product Images
    public ICollection<ProductImageResponseDto> ProductImages {get; set;} = [];
}