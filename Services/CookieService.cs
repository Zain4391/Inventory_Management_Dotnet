
using Inventory_Management_dotnet.Options;
using Inventory_Management_dotnet.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace Inventory_Management_dotnet.Services;

public class CookieService : ICookieService
{
    public readonly IOptions<JwtSettingsOptions> _jwtSettings;
    public readonly IHttpContextAccessor _httpContextAccessor;

    public CookieService(IOptions<JwtSettingsOptions> jwtSettings, IHttpContextAccessor httpContextAccessor)
    {
        _jwtSettings = jwtSettings;
        _httpContextAccessor = httpContextAccessor;
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
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.Value.RefreshTokenExpiryInDays)
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
}