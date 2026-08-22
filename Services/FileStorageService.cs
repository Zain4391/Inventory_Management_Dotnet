using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Inventory_Management_dotnet.Exceptions;
using Inventory_Management_dotnet.Models.DTOs.Request;
using Inventory_Management_dotnet.Models.DTOs.Response;
using Inventory_Management_dotnet.Models.Entities;
using Inventory_Management_dotnet.Options;
using Inventory_Management_dotnet.Repositories.Interfaces;
using Inventory_Management_dotnet.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace Inventory_Management_dotnet.Services;

public class FileStorageService : IFileStorageService
{
    private static readonly Dictionary<string, List<byte[]>> FileSignatures = new()
        {
            { ".jpeg", new() { new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 }, new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 } } },
            { ".jpg",  new() { new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 }, new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 } } },
            { ".png",  new() { new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } } }
        };
    private static readonly List<string> allowedExtension = [".jpg", ".jpeg", ".png"];

    private const int MaxFileSize = 5 * 1024 * 1024;
    private async Task<string> UploadToAzurite(IFormFile File)
    {
        string connectionString = _blobStorageOptions.Value.AzureStorageKey ?? throw new InvalidOperationException("AzureStorageKey is not configured.");
        string containerName = _blobStorageOptions.Value.ContainerName ?? throw new InvalidOperationException("ContainerName is not configured.");
        string extension = Path.GetExtension(File.FileName);

        // Validate the file
        _logger.LogInformation("Validating File extension and signature...");
        ValidateFile(File);

        // Add file typr validation
        string name = Guid.NewGuid().ToString() + $"{extension}";

        // Initialize the client
        _logger.LogInformation("Creating Azure Blob Container Client...");
        BlobContainerClient containerClient = new BlobContainerClient(connectionString, containerName);

        // Make the container if it does not exist
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

        // Get reference to the blob if it exists and delete it
        BlobClient blobClient = containerClient.GetBlobClient(name);
        await blobClient.DeleteIfExistsAsync();

        // Upload the new Blob
        _logger.LogInformation("Uploading file {file}", File.FileName);
        using var fileStream = File.OpenReadStream();
        await blobClient.UploadAsync(
            fileStream,
            new BlobHttpHeaders { ContentType = File.ContentType}
        );

        return blobClient.Uri.ToString();
    }

    private void ValidateFile(IFormFile file)
    {
        string extension = Path.GetExtension(file.FileName);
        if(!allowedExtension.Contains(extension))
        {
            _logger.LogWarning("File extension {file} is wrong", extension);
            throw new BadRequestException("Invalid file extension");
        }

        // Read first 8 bytes of file (file signature)
        using var reader = new BinaryReader(file.OpenReadStream());
        var headerBytes = reader.ReadBytes(8);

        // Get signatures for the extensions
        var signatures = FileSignatures[extension];

        /*
        Compares the uploaded file's signature with list of possible signatures 
        with respect to extension to signature mapping in the Dictionary
        */
        bool isValid = signatures.Any(signature => headerBytes.Length >= signature.Length && headerBytes.Take(signature.Length).SequenceEqual(signature));

        if(!isValid)
        {
            _logger.LogWarning("{File}'s contents do not match the extension", file.FileName);
            throw new BadRequestException("File contents do not match file extension");
        }
    }

    private readonly IProductRepository _productRepository;

    private readonly ILogger<IFileStorageService> _logger;

    private readonly IOptions<BlobStorageOptions> _blobStorageOptions;

    public FileStorageService(IProductRepository productRepository, ILogger<IFileStorageService> logger, IOptions<BlobStorageOptions> blobStorage)
    {
        _productRepository = productRepository;
        _logger = logger;
        _blobStorageOptions = blobStorage;
    }

    public async Task<ProductImageResponseDto> AddImage(ProductImageRequestDto productImageRequestDto, Guid id)
    {
        _logger.LogInformation("Getting Product with ID: {id}", id);

        if(productImageRequestDto.File == null || productImageRequestDto.File.Length == 0)
        {
            _logger.LogWarning("No file uploaded/detected");
            throw new BadRequestException("No File Uploaded");
        }

        if (productImageRequestDto.File.Length > MaxFileSize)
        {
            _logger.LogWarning("Invalid filesize");
            throw new BadRequestException($"File size exceeds maximum allowed size of " +
                        $"{MaxFileSize / 1024 / 1024} MB");
        }

        Product product = await _productRepository.GetProductById(id) ?? throw new NotFoundException("Product", id);

        // get the Image URI from the helper
        _logger.LogInformation("Obtaining public URL for {file}", productImageRequestDto.File.FileName);
        string imageUrl = await UploadToAzurite(productImageRequestDto.File);

        // add image to the product
        _logger.LogInformation("Attaching public URL to product");
        Guid imageId = Guid.NewGuid();
        await _productRepository.AddImage(imageId, imageUrl, product.Id);

        // map to DTO
        ProductImageResponseDto dto = new()
        {
            Id = imageId,
            ImageUrl = imageUrl
        };

        return dto;
    }

    public async Task<List<ProductImageResponseDto>> GetAllImages(Guid id)
    {
        _logger.LogDebug("Getting All Images for {ProductId}", id);
        List<ProductImage> response = await _productRepository.GetAllImages(id);

        // Map to DTO
        List<ProductImageResponseDto> dto = response.Select(i => new ProductImageResponseDto
        {
            Id = i.Id,
            ImageUrl = i.ImageUrl
        }).ToList();

        return dto;
    }
}