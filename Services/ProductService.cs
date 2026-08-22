using Inventory_Management_dotnet.Exceptions;
using Inventory_Management_dotnet.Models.DTOs.Request;
using Inventory_Management_dotnet.Models.DTOs.Response;
using Inventory_Management_dotnet.Models.Entities;
using Inventory_Management_dotnet.Repositories.Interfaces;
using Inventory_Management_dotnet.Services.Interfaces;

namespace Inventory_Management_dotnet.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<IProductService> _logger;

    public ProductService(IProductRepository productRepository, ILogger<IProductService> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<ProductResponseDto> CreateProduct(ProductRequestDto productRequestDto)
    {
        Product newProduct = new()
        {
            Name = productRequestDto.Name,
            Description = productRequestDto.Description,
            Category = productRequestDto.Category,
            Quantity = productRequestDto.Quantity,
            Price = productRequestDto.Price
        };

        _logger.LogInformation("Creating product with the Data: {data}", productRequestDto);

        Product response = await _productRepository.CreateProduct(newProduct);

        _logger.LogInformation("Created Product with ID: {id}", response.Id);

        ProductResponseDto dto = new()
        {
            Id = response.Id,
            Name = response.Name,
            Description = response.Description,
            Category = response.Category,
            Quantity = response.Quantity,
            Price = response.Price
        };

        return dto;
    }

    public async Task<List<ProductResponseDto>> GetAllProducts ()
    {
        _logger.LogInformation($"Getting Products");

        List<Product> products = await _productRepository.GetAllProducts();

        List<ProductResponseDto> response = products
            .Select(product => new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Category = product.Category,
                Quantity = product.Quantity,
                Price = product.Price,

                ProductImages = product.Images.Select(image => new ProductImageResponseDto
                {
                    Id = image.Id,
                    ImageUrl = image.ImageUrl
                }).ToList()
            })
            .ToList();

        return response;
    }

    public async Task<ProductResponseDto> GetProductById (Guid id)
    {
        _logger.LogInformation("Getting Product with ID: {id}", id);

        Product? product = await _productRepository.GetProductById(id);

        if(product is null)
        {
            _logger.LogWarning("Product with {id} was not found", id);
           throw new NotFoundException("Product", id);
        }

        ProductResponseDto response = new()
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Category = product.Category,
            Quantity = product.Quantity,
            Price = product.Price,

            ProductImages = product.Images.Select(image => new ProductImageResponseDto
            {
                Id = image.Id,
                ImageUrl = image.ImageUrl
            }).ToList()
        };

        return response;
    }
}