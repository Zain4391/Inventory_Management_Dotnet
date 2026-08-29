using Microsoft.AspNetCore.Identity;

namespace Inventory_Management_dotnet.Models.Entities;
public enum GENDER
{
    male,
    female
}
// Extends Identity User which has Id, Email, RoleManager
public class AppUser : IdentityUser<Guid>
{
    public required string Name {get; set;}

    public int Age {get; set;}

    public GENDER Gender {get; set;}

    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;

    public ICollection<Order> Orders {get; set;} = [];

    public ICollection<RefreshToken> RefreshTokens {get; set;} = [];
}