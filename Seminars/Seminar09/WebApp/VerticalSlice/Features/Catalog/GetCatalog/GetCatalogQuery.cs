using MediatR;

namespace VerticalSlice.Features.Catalog.GetCatalog;

public class GetCatalogQuery : IRequest<GetCatalogPageModel>
{
    public string? SearchText { get; set; }
    public int? CategoryId { get; set; }
    public string? Sort { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
