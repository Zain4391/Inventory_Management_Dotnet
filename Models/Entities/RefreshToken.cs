
namespace Inventory_Management_dotnet.Models.Entities;

public class RefreshToken
{
    public Guid Id {get; set;}

    public Guid UserId {get; set;}

    public required string TokenHash {get; set;} // store token hash instead of plain Refresh Tokens

    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;

    public DateTime ExpiresAt {get; set;}

    public DateTime? RevokedAt {get; set;}

    // for referencing
    public AppUser User {get; set;} = null!;
}