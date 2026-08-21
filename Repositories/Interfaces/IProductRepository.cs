using Inventory_Management_dotnet.Models.Entities;

namespace Inventory_Management_dotnet.Repositories.Interfaces;

public interface IProductRepository
{
    Task<Product> CreateProduct(Product productRequestDto);

    Task<List<Product>> GetAllProducts();

    Task<Product?> GetProductById(Guid id);

    // Add Images to the Product
    Task<Product?> AddImage(Guid imageId, string url, Guid id);

    Task<List<ProductImage>> GetAllImages(Guid id);
}