using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication1.Entities;

namespace WebApplication1.Models
{
    public class CatalogPageViewModel
    {
        public CatalogFilterViewModel Filter { get; set; } = new();
        public PagedResult<Product> Results { get; set; } = new();
        public IEnumerable<SelectListItem> Categories { get; set; } = Enumerable.Empty<SelectListItem>();

    }
}
