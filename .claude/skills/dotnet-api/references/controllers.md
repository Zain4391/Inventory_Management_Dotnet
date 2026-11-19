# Controllers (C1–C8)

```csharp
[Route("api/products")]   // explicit lowercase plural, not [controller]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    public ProductController(IProductService productService) { _productService = productService; }

    [Authorize(Roles = Roles.Admin)]          // attribute order: [Authorize], verb, limits
    [HttpPost]
    public async Task<ActionResult<ProductResponseDto>> CreateProduct([FromBody] ProductRequestDto productRequestDto)
    {
        var response = await _productService.CreateProduct(productRequestDto);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductResponseDto>> GetById(Guid id)
    {
        var response = await _productService.GetProductById(id);
        return Ok(response);
    }
}
```

- **C1** `[Route("api/products")]` explicit lowercase plural (not `[controller]`) + `[ApiController]`; inherit `ControllerBase`.
- **C2** Return type always `Task<ActionResult<TDto>>`. Attribute order: `[Authorize]`, HTTP verb, limits. Body is usually two lines: call service, return.
- **C3** Typed route constraints (`{id:guid}`); sub-resources nested (`{id:guid}/images`); auth routes are kebab verbs (`sign-up`, `log-in`, `refresh`).
- **C4** Explicit binding: `[FromBody]` JSON, `[FromForm]` files, `[FromQuery]` query objects (`ProductQueryRequest`, `UserQueryRequest`).
- **C5** POST → `CreatedAtAction(nameof(GetById), new { id = response.Id }, response)` (or `Created($"/images/{response.Id}", response)`); GET → `Ok(response)` (list endpoints may inline: `return Ok(await _service.GetAll(query));`); PUT → `Ok(updatedDto)`.
- **C6** No try/catch and no `if(!ModelState.IsValid)` in controllers. Errors flow to `GlobalExceptionHandler`; validation is automatic via `[ApiController]` + DTO DataAnnotations.
- **C7** Authorization per action with `Roles` constants: `[Authorize(Roles = Roles.Admin)]` or `[Authorize(Roles = $"{Roles.Admin},{Roles.Staff}")]`. Anonymous only where intended (read endpoints, auth controller).
- **C8** Upload limits are `private const int MaxFileSize` / `MaxRequestSize` used in `[RequestFormLimits(...)]` and `[RequestSizeLimit(...)]`. Files arrive wrapped in a DTO (`ProductImageRequestDto { required IFormFile File }`), not as a raw parameter.
