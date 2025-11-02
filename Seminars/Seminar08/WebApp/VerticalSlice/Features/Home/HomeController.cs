using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace VerticalSlice.Features.Home;

public class HomeController(IMediator mediator) : Controller
{
    private readonly IMediator mediator = mediator;

    public async Task<IActionResult> Index()
    {
        var result = await mediator.Send(new HomeFeaturedQuery());

        return View(result);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
