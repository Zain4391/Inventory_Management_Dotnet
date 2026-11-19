# Auth and security (A1–A7)

Pieces: ASP.NET Identity; `TokenService` (creates JWTs and refresh tokens, hashes refresh tokens); `CookieService` (sets/deletes auth cookies via `IHttpContextAccessor`); `RefreshTokenRepository` (persist, revoke, revoke-all); `AuthService` (sign-up, log-in, refresh); `JwtSettingsOptions` (`Key`, `Issuer`, `Audience`, `ExpiryInMinutes`, `RefreshTokenExpiryInDays`).

- **A1** `AddIdentity<AppUser, IdentityRole<Guid>>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders()`; `AppUser : IdentityUser<Guid>`.
- **A2** JWT validation in `Program.cs`: JWT Bearer is both default authenticate and challenge scheme; `MapInboundClaims = false` (claim names stay as written, `sub` stays `sub`); validate issuer, audience, lifetime and signing key; `ClockSkew = TimeSpan.Zero`; `NameClaimType = JwtRegisteredClaimNames.Name`, `RoleClaimType = "role"`. The token is read from a cookie:
```csharp
OnMessageReceived = context => {
    if(context.Request.Cookies.TryGetValue("access_token", out var token))
        context.Token = token;
    return Task.CompletedTask;
}
```
- **A3** Access token (`TokenService.CreateToken`) returns `(string token, DateTime ExpiresAt)`. Claims: `sub` (user Id), `email`, `name`, `jti` (new Guid), plus one `"role"` claim per role. HMAC-SHA256 with a symmetric key from `JwtSettings.Key`, via `SecurityTokenDescriptor` + `JsonWebTokenHandler` (not the older `JwtSecurityTokenHandler`).
- **A4** Refresh tokens are opaque, not JWTs: `Convert.ToBase64String(RandomNumberGenerator.GetBytes(64))`. Store only the Base64 SHA-256 hash in `RefreshToken.TokenHash` (unique index); the plain token goes to the client; hash incoming tokens before lookup. Rotate on every use. Replaying a revoked token (`RevokedAt != null`) is treated as an attack: revoke every active token for the user (`RevokeAllTokens`), delete both cookies, throw 401. Identify the user from the stored token's `User` navigation, **never from a client-sent userId**.
- **A5** Cookies:
```csharp
new CookieOptions {
    HttpOnly = true,                 // XSS prevention
    Secure = true,                   // HTTPS only
    SameSite = SameSiteMode.Strict,  // CSRF mitigation
    Expires = ...
};
```
  Literal names `"access_token"` and `"refresh_token"`. Tokens are also returned in the response body so cookie and bearer clients both work. Each flag gets an inline comment naming the attack it mitigates.
- **A6** `public static class Roles { public const string Admin = "Admin"; public const string Staff = "Staff"; }`. Seed roles at startup in `Program.cs` inside a `CreateScope()` block using a scoped `RoleManager` and an `if !RoleExistsAsync` check.
- **A7** Flows:
  - **Sign-up:** create `AppUser` with `UserName = Email` → `_userManager.CreateAsync(user, password)` → on failure join the errors and throw `BadRequestException` → add default role `Roles.Staff` → map to `SignUpResponseDto`.
  - **Log-in:** `FindByEmailAsync` (none → `NotFoundException`) → `CheckPasswordAsync` (fail → `UnauthorizedException("Invalid credentials")`) → `GetRolesAsync` → `CreateToken` → create refresh token, hash it, `SaveRefreshToken(hash, userId)` → `_cookieService.SetTokenCookies(access, refresh)` → return DTO with both tokens and `ExpiresAt`.
  - **Refresh:** hash incoming token → not found / expired → 401; already revoked → replay handling (A4) → otherwise set `RevokedAt = UtcNow` on the current token, issue a new access token and refresh token, save the new hash, reset cookies.
