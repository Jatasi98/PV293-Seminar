using DAL.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PV293WebApplication.Models;

public class CatalogPageViewModel
{
    public CatalogFilterViewModel Filter { get; set; } = new();
    public PagedResult<Product> Results { get; set; } = new();
    public IEnumerable<SelectListItem> Categories { get; set; } = Enumerable.Empty<SelectListItem>();

}
