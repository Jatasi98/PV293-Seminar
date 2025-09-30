using BL.DTOs;
using DAL.Entities;

namespace BL.Services;

public interface ISearchService
{
    Task<SearchResult> SearchAsync(string? query, int? maxPerType = null, CancellationToken ct = default);
    Task<List<Product>> GetFeaturedProducts();
    Task<List<Category>> GetFeaturedCategories();
}
