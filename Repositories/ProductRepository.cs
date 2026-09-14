using Inventory_Management_dotnet.Common;
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

    public async Task<PageResult<Product>> GetAllProducts(ProductQueryRequest productQueryRequest)
    {
        IQueryable<Product> query = _productContext.Products.AsNoTracking();

        // perform search
        if(!string.IsNullOrEmpty(productQueryRequest.Search))
        {
            query = query.Where(p => p.Name == productQueryRequest.Search 
                            || p.Description!.Contains(productQueryRequest.Search));
        }

        if(productQueryRequest.Category.HasValue)
        {
            query = query.Where(p => p.Category == productQueryRequest.Category.Value);
        }

        // Perform min and max filtering
        if(productQueryRequest.MinPrice.HasValue)
        {
            query = query.Where(p => p.Price >= productQueryRequest.MinPrice.Value);
        }

        if(productQueryRequest.MaxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= productQueryRequest.MaxPrice.Value);
        }

        query = productQueryRequest.SortBy?.ToLower() switch
        {
            "price" => productQueryRequest.SortDescending
                ? query.OrderByDescending(p => p.Price)
                : query.OrderBy(p => p.Price),

            "quantity" => productQueryRequest.SortDescending
                ? query.OrderByDescending(p => p.Quantity)
                : query.OrderBy(p => p.Quantity),

            "createdat" => productQueryRequest.SortDescending
                ? query.OrderByDescending(p => p.CreatedAt)
                : query.OrderBy(p => p.CreatedAt),

            "name" => productQueryRequest.SortDescending
                ? query.OrderByDescending(p => p.Name)
                : query.OrderBy(p => p.Name),

            _ => query.OrderBy(p => p.Name)
        };

        var totalCount = await query.CountAsync();
        var products = await query.Skip((productQueryRequest.Page - 1) * productQueryRequest.PageSize)
        .Take(productQueryRequest.PageSize)
        .ToListAsync();

        PageResult<Product> dto = new PageResult<Product>
        {
            Items = products,
            Page = productQueryRequest.Page,
            PageSize = productQueryRequest.PageSize,
            TotalCount = totalCount
        };

        return dto;
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