using Application.DTOs;

namespace Application.Services.Interfaces;
public interface ISearchService
{
    Task<SearchResult> SearchAsync(string? query, int? maxPerType = null, CancellationToken ct = default);
    Task<List<ProductDTO>> GetFeaturedProducts();
    Task<List<CategoryDTO>> GetFeaturedCategories();
}
