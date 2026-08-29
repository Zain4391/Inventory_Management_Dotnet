
using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_dotnet.Models.DTOs.Request;

public class LogInRequestDto
{
    [Required(ErrorMessage = "Email is required")]
    public required string Email {get; set;}

    [Required(ErrorMessage = "Email is required")]
    public required string Password {get; set;}
}