
namespace Inventory_Management_dotnet.Services.Interfaces;

public interface ICookieService
{
    void SetTokenCookies(string accessToken, string refreshToken);
    void RemoveRefreshTokenFromCookie();
    void RemoveAccessTokenFromCookie();

    // The caller's "sub" claim from the validated access token. Throws UnauthorizedException if missing or not a Guid
    Guid GetUserId();

    bool IsAdmin();
}