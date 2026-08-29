using System.ComponentModel.DataAnnotations;
using Inventory_Management_dotnet.Models.Entities;

namespace Inventory_Management_dotnet.Models.DTOs.Request;

public class SignUpRequestDto
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(20, MinimumLength = 5, ErrorMessage = "Name length must be between 5 and 20 characters")]
    public required string Name {get; set;}

    [Required(ErrorMessage = "Email is required")]
    [StringLength(50, MinimumLength = 10, ErrorMessage = "Email length must be between 10 and 50 characters")]
    public required string Email {get; set;}

    [Required(ErrorMessage = "Password is required")]
    [StringLength(16, MinimumLength = 8, ErrorMessage = "Password length must be between 10 and 50 characters")]
    public required string Password {get; set;}

    [Range(1, 100)]
    public int Age {get; set;}

    [Required(ErrorMessage = "Category is required")]
    [EnumDataType(typeof(GENDER), ErrorMessage = "Invalid Category Provided")]
    public GENDER Gender {get; set;}
}