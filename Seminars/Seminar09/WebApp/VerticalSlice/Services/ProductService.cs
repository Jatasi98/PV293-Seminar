using Microsoft.EntityFrameworkCore;
using VerticalSlice.Domain.Entities;
using VerticalSlice.DTOs;
using VerticalSlice.Infrastructure;

namespace VerticalSlice.Services;

public class ProductService(WebAppDbContext db) : IProductService
{
    private readonly WebAppDbContext db = db;

    public async Task<ProductDTO?> GetProduct(int productId)
    {
        var product = await db.Products
            .Include(p => p.Category)
            .AsNoTracking()
            .Select(product => new ProductDTO
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                IsDeleted = product.IsDeleted,
                CategoryId = product.CategoryId,
                CategoryName = product.Category != null ? product.Category.Name : null
            })
            .FirstOrDefaultAsync(p => p.Id == productId);

        return product;
    }

    public async Task<List<ProductDTO>> GetProducts()
    {
        var products = await db.Products
            .AsNoTracking()
            .Select(product => new ProductDTO
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                IsDeleted = product.IsDeleted,
                CategoryId = product.CategoryId,
                CategoryName = product.Category != null ? product.Category.Name : null
            })
            .ToListAsync();

        return products;
    }

    public async Task<ProductDTO?> FindProductById(int id)
    {
        var product = await db.Products
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new ProductDTO
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description,
                IsDeleted = p.IsDeleted,
                CategoryId = p.CategoryId,
                CategoryName = p.Category != null ? p.Category.Name : null
            })
            .FirstOrDefaultAsync();

        if (product == null)
        {
            return null;
        }

        return product;
    }

    public async Task CreateProduct(ProductDTO product)
    {
        if (product == null)
        {
            throw new ArgumentNullException(nameof(product));
        }

        var entity = new Product
        {
            Name = product.Name,
            Price = product.Price,
            Description = product.Description,
            IsDeleted = product.IsDeleted,
            CategoryId = product.CategoryId
        };

        db.Products.Add(entity);
        await db.SaveChangesAsync();

        product.Id = entity.Id;
    }

    public async Task UpdateProduct(ProductDTO product)
    {
        if (product == null)
        {
            throw new ArgumentNullException(nameof(product));
        }

        var entity = await db.Products.FirstOrDefaultAsync(p => p.Id == product.Id);

        if (entity == null)
        {
            throw new KeyNotFoundException("Product not found.");
        }

        entity.Name = product.Name;
        entity.Price = product.Price;
        entity.Description = product.Description;
        entity.IsDeleted = product.IsDeleted;
        entity.CategoryId = product.CategoryId;

        await db.SaveChangesAsync();
    }
}
