using Inventory_Management_dotnet.Data;
using Inventory_Management_dotnet.Models.DTOs.Request;
using Inventory_Management_dotnet.Models.Entities;
using Inventory_Management_dotnet.Options;
using Inventory_Management_dotnet.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Inventory_Management_dotnet.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext _tokenContext;
    private readonly IOptions<JwtSettingsOptions> _jwtSettings;


    public RefreshTokenRepository(AppDbContext tokenContext, IOptions<JwtSettingsOptions> jwtSettings)
    {
        _tokenContext = tokenContext;
        _jwtSettings = jwtSettings;

    }

    public async Task<RefreshToken?> RevokeRefreshToken(RefreshTokenRequestDto refreshTokenRequestDto)
    {
        RefreshToken? existingToken = await _tokenContext.RefreshTokens.Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.TokenHash == refreshTokenRequestDto.RefreshToken);

        if(existingToken is null)
        {
            return null;
        }
        
        // revoke the token
        existingToken!.RevokedAt = DateTime.UtcNow;

        await _tokenContext.SaveChangesAsync();

        return existingToken;
    }

    public async Task<RefreshToken> SaveRefreshToken(string TokenHash, Guid userId)
    {
        RefreshToken refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            TokenHash = TokenHash,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.Value.RefreshTokenExpiryInDays)
        };

        // save the token
        _tokenContext.RefreshTokens.Add(refreshToken);

        await _tokenContext.SaveChangesAsync();
        return refreshToken;
    }
}