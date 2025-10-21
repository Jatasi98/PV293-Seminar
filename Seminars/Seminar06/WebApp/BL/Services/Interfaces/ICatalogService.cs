using BL.DTOs;

namespace BL.Services;

public interface ICatalogService
{
    Task<SearchResult> GetCatalogResults(
            string? search,
            int? categoryId,
            string? sort,
            int page,
            int pageSize);
}
