using Application.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Presentation.Models;

public class CatalogPageViewModel
{
    public CatalogFilterViewModel Filter { get; set; } = new();
    public PagedResult<ProductDTO> Results { get; set; } = new();
    public IEnumerable<SelectListItem> Categories { get; set; } = [];
}
