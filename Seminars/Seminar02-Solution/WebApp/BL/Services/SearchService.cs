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
            return new SearchResult("", [], []);
        }

        query = query.Trim();

        var products = await _db.Products
            .Include(p => p.Category)
            .AsNoTracking()
            .Where(p => p.Name.Contains(query) || p.Description != null && p.Description.Contains(query))
            .ToListAsync(ct);

        var categories = await _db.Categories
            .AsNoTracking()
            .Where(c => c.Name.Contains(query) || c.Description != null && c.Description.Contains(query))
            .ToListAsync(ct);

        return new SearchResult(query, products, categories);
    }

    public async Task<List<Product>> GetFeaturedProducts()
    {
        var products = await _db.Products
            .Include(p => p.Category)
            .OrderByDescending(p => p.CreatedOnUTC)
            .Take(Constants.FeaturedProductCount)
            .AsNoTracking()
            .ToListAsync();

        return products;
    }

    public async Task<List<Category>> GetFeaturedCategories()
    {
        var categories = await _db.Categories
            .OrderBy(c => c.Id)
            .Take(Constants.FeaturedCategoryCount)
            .AsNoTracking()
            .ToListAsync();

        return categories;
    }
}
