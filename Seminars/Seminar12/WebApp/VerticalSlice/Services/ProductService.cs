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
                Price = product.Price.Value,
                Currency = product.Price.Currency,
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
                Price = product.Price.Value,
                Currency = product.Price.Currency,
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
            .Select(product => new ProductDTO
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price.Value,
                Currency = product.Price.Currency,
                Description = product.Description,
                IsDeleted = product.IsDeleted,
                CategoryId = product.CategoryId,
                CategoryName = product.Category != null ? product.Category.Name : null
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

        var entity = Product.Create(product.Name, product.Description, Money.Create(product.Price, product.Currency), product.CategoryId);

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

        if (!string.IsNullOrWhiteSpace(product.Description))
        {
            entity.UpdateDescription(product.Description);
        }
        
        if (product.Price != entity.Price.Value || product.Currency != entity.Price.Currency)
        {
            entity.ChangePrice(Money.Create(product.Price, product.Currency));
        }

        if (product.CategoryId != entity.CategoryId)
        {
            entity.ChangeCategory(product.CategoryId);
        }  

        await db.SaveChangesAsync();
    }
}
