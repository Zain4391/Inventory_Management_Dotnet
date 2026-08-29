using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Inventory_Management_dotnet.Models.Entities;
using Inventory_Management_dotnet.Options;
using Inventory_Management_dotnet.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Inventory_Management_dotnet.Services;

public class TokenService : ITokenService
{
    private readonly IOptions<JwtSettingsOptions> _jwtSettings;

    public TokenService(IOptions<JwtSettingsOptions> jwtSettings)
    {
        _jwtSettings = jwtSettings;
    }

    public (string token, DateTime ExpiresAt) CreateToken(AppUser User, IEnumerable<string> roles)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.Value.ExpiryInMinutes);

        // build claims that the token will hold
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, User.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, User.Email!),
            new(JwtRegisteredClaimNames.Name, User.Name),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // add roles
        claims.AddRange(roles.Select(role => new Claim("role", role)));

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Value.Key));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = _jwtSettings.Value.Issuer,
            Expires = expiresAt,
            Audience = _jwtSettings.Value.Audience,
            SigningCredentials = credentials
        };

        var handler = new JsonWebTokenHandler();
        var token = handler.CreateToken(descriptor);

        return (token, expiresAt);
    }

    // Refresh tokens are just random strings
    // They're not JWTs
    public string CreateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    public string HashRefreshToken(string token)
    {
        var bytes = SHA256.HashData(
            Encoding.UTF8.GetBytes(token)
        );

        return Convert.ToBase64String(bytes);
    }
}