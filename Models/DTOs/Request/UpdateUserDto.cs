using System.ComponentModel.DataAnnotations;
using Inventory_Management_dotnet.Models.Entities;

namespace Inventory_Management_dotnet.Models.DTOs.Request;

public class UpdateUserRequestDto
{
    [StringLength(20, MinimumLength = 5, ErrorMessage = "Name length must be between 5 and 20 characters")]
    public string? Name {get; set;}

    [StringLength(50, MinimumLength = 10, ErrorMessage = "Email length must be between 10 and 50 characters")]
    public string? Email {get; set;}

    [Range(1, 100)]
    public int? Age {get; set;}

    [EnumDataType(typeof(GENDER), ErrorMessage = "Invalid Category Provided")]
    public GENDER? Gender {get; set;}
}