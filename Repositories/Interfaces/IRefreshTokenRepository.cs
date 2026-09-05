using Inventory_Management_dotnet.Models.DTOs.Request;
using Inventory_Management_dotnet.Models.Entities;

namespace Inventory_Management_dotnet.Repositories.Interfaces;

public interface IRefreshTokenRepository
{
    // Will be used by the Refresh method and endpoint
    Task<RefreshToken?> RevokeRefreshToken(RefreshTokenRequestDto refreshTokenRequestDto);

    // Will be used by the Login Method
    Task<RefreshToken> SaveRefreshToken(string TokenHash, Guid userId);

    Task RevokeAllTokens(Guid userId);

    Task<RefreshToken?> GetRefreshToken(string tokenHash);
}
