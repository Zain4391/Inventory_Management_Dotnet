using Inventory_Management_dotnet.Models.DTOs.Request;
using Inventory_Management_dotnet.Models.DTOs.Response;
using Inventory_Management_dotnet.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Management_dotnet.Controllers;

[Route("api/products")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IFileStorageService _fileStorageService;

    // File upload checks
    private const int MaxFileSize = 5 * 1024 * 1024;
    private const int MaxRequestSize = 10 * 1024 * 1024;

    public ProductController(IProductService productService, IFileStorageService fileStorageService)
    {
        _productService = productService;
        _fileStorageService = fileStorageService;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductResponseDto>> GetById(Guid id)
    {
        var response = await _productService.GetProductById(id);
        return Ok(response);
    }

    [HttpGet("{id:guid}/images")]
    public async Task<ActionResult<ProductImageResponseDto>> GetProductImages(Guid id)
    {
        var response = await _fileStorageService.GetAllImages(id);
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<ProductResponseDto>> CreateProduct([FromBody] ProductRequestDto productRequestDto)
    {
        if(!ModelState.IsValid)
        {
            return BadRequest();
        }
        var response = await _productService.CreateProduct(productRequestDto);
        return CreatedAtAction(
            nameof(GetById),
            new { id = response.Id },
            response
        );
    }

    [HttpPost("{id:guid}/images")]
    [RequestFormLimits(
        MultipartBodyLengthLimit = MaxFileSize,
        ValueLengthLimit = MaxRequestSize
    )]
    [RequestSizeLimit(MaxRequestSize)]
    public async Task<ActionResult<ProductImageResponseDto>> CreateProductImage(Guid id, [FromForm] ProductImageRequestDto productImageRequestDto)
    {
        var response = await _fileStorageService.AddImage(productImageRequestDto, id);
        return Created($"/images/{response.Id}",response);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetAllProducts()
    {
        return Ok(await _productService.GetAllProducts());
    }

}