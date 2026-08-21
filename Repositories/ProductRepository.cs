using Inventory_Management_dotnet.Data;
using Inventory_Management_dotnet.Models.Entities;
using Inventory_Management_dotnet.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Management_dotnet.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _productContext;

    public ProductRepository(AppDbContext productDbContext)
    {
        _productContext = productDbContext;
    }

    // Method Implementations
    public async Task<Product> CreateProduct(Product product)
    {
        Guid id = Guid.NewGuid();
        Product newProduct = new()
        {
            Id = id,
            Name = product.Name,
            Description = product.Description,
            Category = product.Category,
            Quantity = product.Quantity,
            Price = product.Price
        };

        // Save 
        await _productContext.AddAsync(newProduct);
        await _productContext.SaveChangesAsync();
        return newProduct;
    }

    public async Task<List<Product>> GetAllProducts()
    {
        List<Product> products = await _productContext.Products.Include(p => p.Images).ToListAsync();
        return products;
    }

    public async Task<Product?> GetProductById(Guid id)
    {
        Product? product = await _productContext.Products
                                .Include(p => p.Images)
                                .FirstOrDefaultAsync(p => p.Id == id);
        return product;
    }

    public async Task<Product?> AddImage(Guid imageId, string url, Guid id)
    {
        // check if product exists
        Product? product = await GetProductById(id);
        // create Image for the product
        ProductImage image = new()
        {
            Id = imageId,
            ImageUrl = url,
            ProductId = id
        };

        // Add it to the product
        await _productContext.ProductImages.AddAsync(image);

        // Persist to the Database
        await _productContext.SaveChangesAsync();

        return product;
    }

    public async Task<List<ProductImage>> GetAllImages(Guid id)
    {
        List<ProductImage> productImages = await _productContext.ProductImages.Where(pi => pi.ProductId == id).ToListAsync();  
        return productImages;
    }
}