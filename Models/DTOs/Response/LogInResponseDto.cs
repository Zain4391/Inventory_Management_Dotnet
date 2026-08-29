
namespace Inventory_Management_dotnet.Models.DTOs.Response;

public class LogInResponseDto
{
    public required string Access_token {get; set;}

    public required string Refresh_token {get; set;}

    public DateTime ExpiresAt {get; set;}
}