using Inventory_Management_dotnet.Models.DTOs.Request;
using Inventory_Management_dotnet.Models.DTOs.Response;

namespace Inventory_Management_dotnet.Services.Interfaces;

public interface IFileStorageService
{
    Task<ProductImageResponseDto> AddImage(ProductImageRequestDto productImageRequestDto, Guid id);

    Task<List<ProductImageResponseDto>> GetAllImages(Guid id);
}