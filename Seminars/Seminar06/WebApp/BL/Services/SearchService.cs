using BL.DTOs;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace BL.Services;

public class SearchService(WebAppDbContext db) : ISearchService
{
    private readonly WebAppDbContext _db = db;

    public async Task<SearchResult> SearchAsync(string? query, int? maxPerType = null, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return new SearchResult();
        }

        query = query.Trim();

        var products = await _db.Products
            .Include(p => p.Category)
            .AsNoTracking()
            .Where(p => p.Name.Contains(query) || p.Description != null && p.Description.Contains(query))
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
            .ToListAsync(ct);

        var categories = await _db.Categories
            .AsNoTracking()
            .Where(c => c.Name.Contains(query) || c.Description != null && c.Description.Contains(query))
            .Select(category => new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            })
            .ToListAsync(ct);

        return new SearchResult()
        {
            Query = query,
            Products = new PagedResult<ProductDTO>
            {
                Items = products,
            },
            Categories = categories,
        };
    }

    public async Task<List<ProductDTO>> GetFeaturedProducts()
    {
        var products = await _db.Products
            .Include(p => p.Category)
            .OrderByDescending(p => p.CreatedOnUTC)
            .Take(Constants.FeaturedProductCount)
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

    public async Task<List<CategoryDTO>> GetFeaturedCategories()
    {
        var categories = await _db.Categories
            .OrderBy(c => c.Id)
            .Take(Constants.FeaturedCategoryCount)
            .AsNoTracking()
            .Select(category => new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            })
            .ToListAsync();

        return categories;
    }
}
