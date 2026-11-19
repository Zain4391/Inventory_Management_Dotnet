# Services (S1–S9)

```csharp
public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<IProductService> _logger;     // typed on the INTERFACE
    public ProductService(IProductRepository productRepository, ILogger<IProductService> logger)
    { _productRepository = productRepository; _logger = logger; }

    public async Task<ProductResponseDto> GetProductById(Guid id)
    {
        _logger.LogInformation("Getting Product with ID: {id}", id);        // 1 log intent

        Product? product = await _productRepository.GetProductById(id);     // 2 repo call, explicit type

        if(product is null)                                                 // 3 guard -> log -> throw
        {
            _logger.LogWarning("Product with {id} was not found", id);
            throw new NotFoundException("Product", id);
        }

        ProductResponseDto response = new()                                 // 4 manual mapping
        {
            Id = product.Id,
            ProductImages = product.Images.Select(image => new ProductImageResponseDto
            {
                Id = image.Id,
                ImageUrl = image.ImageUrl
            }).ToList()
        };

        return response;                                                    // 5 return DTO
    }
}
```

- **S1** Every method follows the 5-step shape. Method names have **no `Async` suffix** (`CreateProduct`, `GetProductById`, `LogIn`). Methods take request DTOs and/or a `Guid` and return response DTOs, never entities. Services may depend on repositories, framework services (`UserManager<AppUser>`, `IOptions<T>`, `IHttpContextAccessor`) and other services (`ITokenService`, `ICookieService`).
- **S2** Errors are **thrown** as `AppException` subclasses. No Result types; never return null or error codes to the controller.
- **S3** Null checks: `is null` block (log then throw) or the one-liner `await repo.X(id) ?? throw new NotFoundException("Product", id);`.
- **S4** Log levels: Information for steps, Warning for client mistakes (not found, invalid input), Error for failed operations. Structured templates (`{id}`), never `$"..."` interpolation.
- **S5** Mapping is manual, field by field: inline `new XDto { ... }`, `.Select(x => new XDto { ... }).ToList()` for child collections. No AutoMapper, no mapper extension methods. Don't duplicate the same mapping across methods; extract a private helper in the service.
- **S6** Pagination bounds are checked in the service, then the result is re-wrapped as `PageResult<TDto>` keeping Page/PageSize/TotalCount from the repository's `PageResult<TEntity>`:
```csharp
if(req.Page < 1) throw new BadRequestException("Page cannot be negative");
if(req.PageSize < 1 || req.PageSize > 100) throw new BadRequestException("Page must be between 1 and 100");
```
- **S7** Partial updates (PUT with optional fields): nullable request fields; apply each only if `HasValue` / `!= null`; log each change. Identity operations (`SetEmailAsync`, `UpdateAsync`, `CreateAsync`): check `result.Succeeded`, then join descriptions into a `BadRequestException` or throw `ValidationException(code, description)`.
- **S8** Private helper methods for internal steps (`RevokeAllTokens`, `UploadToAzurite`, `ValidateFile`); static readonly lookups/constants on the class (`FileSignatures`, `allowedExtension`, `MaxFileSize`); step comments (`// hash the token to store it in DB`, `// map to DTO`).
- **S9** Request DTO → entity via object initializer in the service (`Product newProduct = new() { Name = dto.Name, ... };`). IDs via `Guid.NewGuid()` right before saving (service or repository). Timestamps from the entity default (`= DateTime.UtcNow`) or set explicitly.
