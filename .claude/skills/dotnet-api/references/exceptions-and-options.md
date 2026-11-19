# Exceptions (X1–X3) and Options (O1–O4)

## Exceptions
```
Exception
└── abstract AppException            (message, HttpStatusCode = 500)
    ├── sealed BadRequestException   → 400
    ├── sealed UnauthorizedException → 401
    ├── sealed NotFoundException     → 404   (resourceName, key)
    ├── sealed ConflictException     → 409   (use for duplicates)
    └── sealed ValidationException   → 400   + Errors dictionary
```
- **X1** The base carries the status; each subclass is a sealed one-liner:
```csharp
public abstract class AppException : Exception
{
    public HttpStatusCode statusCode {get;}      // camelCase property
    public AppException(string message, HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError)
        : base(message) { statusCode = httpStatusCode; }
}
public sealed class BadRequestException : AppException
{
    public BadRequestException(string message) : base(message, HttpStatusCode.BadRequest) {}
}
```
  `NotFoundException(string resourceName, object key)` builds `"{resourceName} with ID: {key} not found"`. `ValidationException` has a fixed message `"One or more validation errors occurred."` and an `IDictionary<string, string[]> Errors`, with two constructors (full dictionary, or a single field/error pair).
- **X2** `GlobalExceptionHandler : IExceptionHandler` (sealed), registered with `AddProblemDetails()`, `AddExceptionHandler<GlobalExceptionHandler>()`, `app.UseExceptionHandler()`. Order: (1) log `AppException` as Warning ("Handled exception"), anything else as Error; (2) map via tuple switch:
```csharp
var (statusCode, title) = exception switch
{
    AppException appEx => ((int)appEx.statusCode, ReasonPhrases.GetReasonPhrase((int)appEx.statusCode)),
    _ => (StatusCodes.Status500InternalServerError, "Internal Server Error")
};
```
  (3) build RFC 7807 `ProblemDetails`: Status, Title, Detail = exception message (generic text on 500 so internals don't leak), Instance = request path, extension `traceId` = `HttpContext.TraceIdentifier`, extension `errors` only for `ValidationException`; (4) set status, write JSON, return `true`.
- **X3** Throw only from services (never repositories/controllers). Standard pattern is log then throw (short `?? throw` form when no log is needed). Client errors → `AppException` subclasses (4xx); startup/config errors → built-in `InvalidOperationException` (hidden 500). Typical use: `BadRequestException` for input DataAnnotations can't express (page bounds, bad file, Identity create failure); `NotFoundException` when the repository returned null; `UnauthorizedException` for auth failures (wrong password; refresh token missing/expired/replayed); `ValidationException` for Identity `UpdateAsync` errors (`new ValidationException(error.Code, error.Description)`).

## Options
```csharp
namespace Inventory_Management_dotnet.Options;   // use the project's namespace

public class JwtSettingsOptions
{
    public const string SectionName = "JwtSettings";        // section name lives on the class

    [Required(AllowEmptyStrings = false)]
    public string Key {get; set;} = string.Empty;           // can be placed in user-secrets

    [Required(AllowEmptyStrings = false)]
    public string Issuer {get; set;} = string.Empty;

    public int ExpiryInMinutes {get; set;}
    public int RefreshTokenExpiryInDays {get; set;}
}
```
- **O1** `XxxOptions` classes in `Options/`; `public const string SectionName` matches the top-level appsettings key; strings `= string.Empty` with `[Required(AllowEmptyStrings = false)]`; numeric settings carry no validation attributes (add `[Range]` if a bad value would be dangerous).
- **O2** Register every options class so a missing setting stops the app at startup:
```csharp
builder.Services.AddOptions<JwtSettingsOptions>()
    .BindConfiguration(JwtSettingsOptions.SectionName)
    .ValidateDataAnnotations()
    .ValidateOnStart();
```
- **O3** Consume via `IOptions<T>`, stored as `_jwtSettings` / `_blobStorageOptions`, reading `.Value.Prop` where needed. Only `Program.cs` (before DI exists) uses `builder.Configuration.GetSection(T.SectionName).Get<T>() ?? throw new InvalidOperationException("... not configured.")`. Don't re-null-check option values already validated by `ValidateOnStart`. `IOptionsMonitor`/`IOptionsSnapshot` aren't used (settings are read once).
- **O4** The connection string is not an options class: `GetConnectionString("DbConnectionString")`. `appsettings.json` holds placeholders only (`"<YOUR_AZURE_STORAGE_KEY>"`, `"<YOUR_256_bit_key_here>"`); real secrets go in development settings / user-secrets / environment. Sections: Serilog, BlobStorage, ConnectionStrings, JwtSettings.
