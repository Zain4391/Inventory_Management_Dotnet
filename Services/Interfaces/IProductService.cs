using Inventory_Management_dotnet.Common;
using Inventory_Management_dotnet.Models.DTOs.Request;
using Inventory_Management_dotnet.Models.DTOs.Response;

namespace Inventory_Management_dotnet.Services.Interfaces;

public interface IProductService
{
    Task<ProductResponseDto> CreateProduct(ProductRequestDto productRequestDto);

    Task<PageResult<ProductResponseDto>> GetAllProducts(ProductQueryRequest productQueryRequest);

    Task<ProductResponseDto> GetProductById(Guid id);
}