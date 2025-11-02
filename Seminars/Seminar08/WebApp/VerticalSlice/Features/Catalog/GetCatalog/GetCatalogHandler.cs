using MediatR;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VerticalSlice.DTOs;
using VerticalSlice.Infrastructure;

namespace VerticalSlice.Features.Catalog.GetCatalog;

public class GetCatalogHandler : IRequestHandler<GetCatalogQuery, GetCatalogPageModel>
{
    private readonly WebAppDbContext dbContext;

    public GetCatalogHandler(WebAppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<GetCatalogPageModel> Handle(GetCatalogQuery request, CancellationToken cancellationToken)
    {
        if (request.Page <= 0)
        {
            request.Page = 1;
        }

        if (request.PageSize <= 0 || request.PageSize > 100)
        {
            request.PageSize = 12;
        }

        var queryable = dbContext.Products
            .AsNoTracking()
            .Include(product => product.Category)
            .Where(product => !product.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var searchText = request.SearchText.Trim();
            queryable = queryable.Where(product =>
                product.Name.Contains(searchText) ||
                (product.Description != null && product.Description.Contains(searchText)));
        }

        if (request.CategoryId.HasValue)
        {
            queryable = queryable.Where(product => product.CategoryId == request.CategoryId);
        }

        queryable = request.Sort switch
        {
            "name_desc" => queryable.OrderByDescending(product => product.Name),
            "price_asc" => queryable.OrderBy(product => product.Price).ThenBy(product => product.Name),
            "price_desc" => queryable.OrderByDescending(product => product.Price).ThenBy(product => product.Name),
            _ => queryable.OrderBy(product => product.Name),
        };

        var products = await queryable
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
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

        var categories = await dbContext.Categories
            .AsNoTracking()
            .OrderBy(category => category.Name)
            .Select(category => new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            })
            .ToListAsync();

        return new GetCatalogPageModel
        {
            Results = new PagedResult<ProductDTO>
            {
                Items = products,
                TotalItems = products.Count,
                Page = request.Page,
                PageSize = request.PageSize,
            },
            Categories = [.. categories
                .Select(category => new SelectListItem
                {
                    Value = category.Id.ToString(),
                    Text = category.Name
                })]
        };
    }
}
