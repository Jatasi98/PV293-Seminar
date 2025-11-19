using Microsoft.AspNetCore.Mvc.Rendering;
using VerticalSlice.DTOs;

namespace VerticalSlice.Features.Catalog.GetCatalog;

public class GetCatalogPageModel
{
    public GetCatalogFilterModel Filter { get; set; } = new();
    public PagedResult<ProductDTO> Results { get; set; } = new();
    public IEnumerable<SelectListItem> Categories { get; set; } = [];
}
