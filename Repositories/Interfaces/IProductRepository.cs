using Inventory_Management_dotnet.Common;
using Inventory_Management_dotnet.Models.Entities;

namespace Inventory_Management_dotnet.Repositories.Interfaces;

public interface IProductRepository
{
    Task<Product> CreateProduct(Product productRequestDto);

    Task<PageResult<Product>> GetAllProducts(ProductQueryRequest pageQueryRequest);

    Task<Product?> GetProductById(Guid id);

    // Loads several products in one query (WHERE Id IN (...)), missing ids are simply absent
    Task<List<Product>> GetProductsByIds(IEnumerable<Guid> ids);

    // Add Images to the Product
    Task<Product?> AddImage(Guid imageId, string url, Guid id);

    Task<List<ProductImage>> GetAllImages(Guid id);
}