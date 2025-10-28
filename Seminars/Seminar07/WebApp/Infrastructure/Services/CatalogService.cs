using Application.DTOs;
using Application.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class CatalogService : ICatalogService
{
    private readonly WebAppDbContext _db;

    public CatalogService(WebAppDbContext db)
    {
        _db = db;
    }

    public async Task<SearchResult> GetCatalogResults(
            string? search,
            int? categoryId,
            string? sort,
            int page,
            int pageSize)
    {
        if (page <= 0)
        {
            page = 1;
        }

        if (pageSize <= 0 || pageSize > 100)
        {
            pageSize = 12;
        }

        var queryable = _db.Products
            .AsNoTracking()
            .Include(product => product.Category)
            .Where(product => !product.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchText = search.Trim();
            queryable = queryable.Where(product =>
                product.Name.Contains(searchText) ||
                (product.Description != null && product.Description.Contains(searchText)));
        }

        if (categoryId.HasValue)
        {
            queryable = queryable.Where(product => product.CategoryId == categoryId);
        }

        queryable = sort switch
        {
            "name_desc" => queryable.OrderByDescending(product => product.Name),
            "price_asc" => queryable.OrderBy(product => product.Price).ThenBy(product => product.Name),
            "price_desc" => queryable.OrderByDescending(product => product.Price).ThenBy(product => product.Name),
            _ => queryable.OrderBy(product => product.Name),
        };

        var products = await queryable
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
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

        var categories = await _db.Categories
            .AsNoTracking()
            .OrderBy(category => category.Name)
            .Select(category => new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            })
            .ToListAsync();

        return new SearchResult
        {
            Query = search?.Trim() ?? string.Empty,
            Products = new PagedResult<ProductDTO>
            {
                Items = products,
                TotalItems = products.Count,
                Page = page,
                PageSize = pageSize,
            },
            Categories = categories
        };
    }
}
