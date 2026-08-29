
using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_dotnet.Models.DTOs.Request;

// Refresh Token sent here will be the expired one. 
// This DTO will be used to issue a new Refresh Token
public class RefreshTokenRequestDto
{
    [Required(ErrorMessage = "Refresh Token is required")]
    public required string RefreshToken {get; set;}

    [Required(ErrorMessage = "Refresh Token must belong to a user")]
    public Guid UserId {get; set;}
}