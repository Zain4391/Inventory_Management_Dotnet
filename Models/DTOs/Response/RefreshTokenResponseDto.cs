
namespace Inventory_Management_dotnet.Models.DTOs.Response;

// This refresh token is the newly issued one
public class RefreshTokenResponseDto
{
    public required string RefreshToken {get; set;}
    public required string AccessToken {get; set;}
}