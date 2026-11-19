# DTOs (D1–D6) and file validation (V1)

- **D1** Names and places:

| Kind | Folder | Name |
|---|---|---|
| Request DTO | `Models/DTOs/Request/` | `XxxRequestDto` |
| Response DTO | `Models/DTOs/Response/` | `XxxResponseDto` |
| Query object | `Common/` | `XxxQueryRequest` |
| Generic wrapper | `Common/` | `PageResult<T>` |
| Entity | `Models/Entities/` | plain noun |

  Name DTOs by operation (`SignUp...`, `LogIn...`, `UpdateUser...`, `RefreshToken...`); one request/response pair per operation. File name matches the class name.
- **D2** Plain mutable classes with `{get; set;}`; no records. `required` on non-nullable reference types (`required string Name`); not on value types/enums. Optional values nullable (`string?`, `int?`, `GENDER?`). Child collections `= []`. Responses nest other response DTOs (Product → `ProductImageResponseDto`, User → `OrderResponseDto`).
- **D3** DTOs reuse the entity enums (`CATEGORY`, `GENDER`, `STATUS`) directly (`using ...Models.Entities`).
- **D4** DataAnnotations **only on request DTOs** (no FluentValidation):
```csharp
public class ProductRequestDto
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(20, MinimumLength = 5, ErrorMessage = "Invalid message length")]
    public required string Name {get; set;}

    [StringLength(100)]
    public string? Description {get; set;}

    [Required(ErrorMessage = "Category is required")]
    [EnumDataType(typeof(CATEGORY), ErrorMessage = "Invalid Category Provided")]
    public CATEGORY Category {get; set;}

    [Range(1, int.MaxValue)]
    public int Quantity {get; set;}

    [Range(typeof(decimal), "0.01", "999999999")]
    public decimal Price {get; set;}
}
```
  Mandatory → `[Required(ErrorMessage = "X is required")]` together with C# `required`. String length → `[StringLength(max, MinimumLength = min, ErrorMessage = "...")]` (typical: Name 5–20, Email 10–50, Password 8–16). Numeric → `[Range(min, max)]`. Enums → `[EnumDataType(typeof(E), ErrorMessage = "Invalid X Provided")]`. PATCH-style updates → same attributes without `[Required]` on nullable properties. Error messages must describe the actual rule on the actual field.
- **D5** Validation split: `[ApiController]` auto-validates annotations before the action runs and returns a framework `ValidationProblemDetails` 400 (it does not go through the exception handler). Rules that depend on business logic run in the service and throw: page bounds, file size/extension/magic bytes, Identity password rules (`IdentityResult` → `BadRequestException` / `ValidationException`). Query objects have no annotations.
- **D6** Short purpose comments where meaning isn't obvious, e.g. `// Refresh Token sent here will be the expired one.` / `// This refresh token is the newly issued one.`

## File validation
- **V1** In the storage service: extension whitelist, then magic-byte signature check from a static extension→byte-header dictionary, plus empty-file and max-size checks; throw `BadRequestException`. Uploaded files are wrapped in a request DTO (`public class ProductImageRequestDto { public required IFormFile File {get; set;} }`).
