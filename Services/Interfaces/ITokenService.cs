using Inventory_Management_dotnet.Models.Entities;

namespace Inventory_Management_dotnet.Services.Interfaces;

public interface ITokenService
{
    public (string token, DateTime ExpiresAt) CreateToken(AppUser User, IEnumerable<string> roles);
}