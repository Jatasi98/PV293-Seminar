using System.ComponentModel.DataAnnotations;

namespace VerticalSlice.Features.Catalog.GetCatalog;

public class GetCatalogFilterModel
{
    public string? Search { get; set; }

    [Display(Name = "Category")]
    public int? CategoryId { get; set; }

    public string Sort { get; set; } = "name_asc";

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 12;
}
