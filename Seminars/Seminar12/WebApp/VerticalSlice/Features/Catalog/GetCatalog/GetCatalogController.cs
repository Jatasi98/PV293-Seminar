using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace VerticalSlice.Features.Catalog.GetCatalog;

public class GetCatalogController : Controller
{
    private readonly IMediator mediator;

    public GetCatalogController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    public async Task<IActionResult> Index([FromQuery] GetCatalogFilterModel filter)
    {

        var result = await mediator.Send(new GetCatalogQuery()
        {
            CategoryId = filter.CategoryId,
            Page = filter.Page,
            PageSize = filter.PageSize,
            SearchText = filter.Search,
            Sort = filter.Sort,
        });

        if (result is null)
        {
            return NotFound();
        }

        result.Filter = filter;
        return View(result);
    }
}
