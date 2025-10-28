using Application.DTOs;

namespace Application.Services.Interfaces;

public interface ICatalogService
{
    Task<SearchResult> GetCatalogResults(
            string? search,
            int? categoryId,
            string? sort,
            int page,
            int pageSize);
}
