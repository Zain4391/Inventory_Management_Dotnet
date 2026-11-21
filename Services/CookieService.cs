
using System.Security.Claims;
using Inventory_Management_dotnet.Exceptions;
using Inventory_Management_dotnet.Models.Entities;
using Inventory_Management_dotnet.Options;
using Inventory_Management_dotnet.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Inventory_Management_dotnet.Services;

public class CookieService : ICookieService
{
    private readonly IOptions<JwtSettingsOptions> _jwtSettings;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<ICookieService> _logger;

    public CookieService(IOptions<JwtSettingsOptions> jwtSettings, IHttpContextAccessor httpContextAccessor, ILogger<ICookieService> logger)
    {
        _jwtSettings = jwtSettings;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public void SetTokenCookies(string accessToken, string refreshToken)
    {
        var jwtcookieOptions = new CookieOptions
        {
            HttpOnly = true, // XSS prevention
            Secure = true, // HHTPS only
            SameSite = SameSiteMode.Strict, // CSRF mitigation
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.Value.ExpiryInMinutes)
        };

        var refreshTokencookieOptions = new CookieOptions
        {
            HttpOnly = true, // XSS prevention
            Secure = true, // HHTPS only
            SameSite = SameSiteMode.Strict, // CSRF mitigation
            Expires = DateTime.UtcNow.AddDays(_jwtSettings.Value.RefreshTokenExpiryInDays)
        };

        // append cookies
        var response = _httpContextAccessor.HttpContext!.Response;
        response.Cookies.Append("access_token", accessToken, jwtcookieOptions);
        response.Cookies.Append("refresh_token", refreshToken, refreshTokencookieOptions);
    }

    public void RemoveRefreshTokenFromCookie()
    {
        _httpContextAccessor.HttpContext!.Response.Cookies.Delete("refresh_token");
    }

    public void RemoveAccessTokenFromCookie()
    {
        _httpContextAccessor.HttpContext!.Response.Cookies.Delete("access_token");
    }

    public Guid GetUserId()
    {
        /*
            MapInboundClaims = false keeps the JWT claim names as issued,
            so the user id is "sub" (set by TokenService), not ClaimTypes.NameIdentifier.
        */
        string? sub = _httpContextAccessor.HttpContext?.User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if(!Guid.TryParse(sub, out Guid userId))
        {
            _logger.LogError("Access token has invalid or missing claim.");
            throw new UnauthorizedException("Invalid access token");
        }

        return userId;
    }

    // RoleClaimType = "role" in Program.cs, so IsInRole reads the token's role claims
    public bool IsAdmin()
    {
        return _httpContextAccessor.HttpContext?.User.IsInRole(Roles.Admin) ?? false;
    }
}
