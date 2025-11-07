using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace VerticalSlice.Features.Search.GetSearch;

public class GetSearchController(IMediator mediator) : Controller
{
    private readonly IMediator mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery(Name = "q")] string? query)
    {
        var result = await mediator.Send(new GetSearchQuery() { Query = query });

        return View(result);
    }
}
