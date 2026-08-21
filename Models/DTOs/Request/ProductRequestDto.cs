using System.ComponentModel.DataAnnotations;
using Inventory_Management_dotnet.Models.Entities;

namespace Inventory_Management_dotnet.Models.DTOs.Request;

public class ProductRequestDto
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(20, MinimumLength = 5, ErrorMessage = "Invalid message length")]
    public required string Name {get; set;}

    [StringLength(100)]
    public string? Description {get; set;}

    [Required(ErrorMessage = "Category is required")]
    [EnumDataType(typeof(CATEGORY), ErrorMessage = "Invalid Category Provided")]
    public CATEGORY Category {get; set;}

    [Range(1, int.MaxValue)]
    public int Quantity {get; set;}

    [Range(typeof(decimal), "0.01", "999999999")]
    public decimal Price {get; set;}

}