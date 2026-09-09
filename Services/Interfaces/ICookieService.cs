
namespace Inventory_Management_dotnet.Services.Interfaces;

public interface ICookieService
{
    void SetTokenCookies(string accessToken, string refreshToken);
    void RemoveRefreshTokenFromCookie();
    void RemoveAccessTokenFromCookie();
}