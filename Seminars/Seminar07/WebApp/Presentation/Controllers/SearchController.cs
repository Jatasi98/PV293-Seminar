using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;

namespace Presentation.Controllers;

public class SearchController(ISearchService searchService) : Controller
{
    private readonly ISearchService _searchService = searchService;

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery(Name = "q")] string? query)
    {
        var result = await _searchService.SearchAsync(query);

        var model = new SearchViewModel
        {
            Query = query ?? string.Empty,
            Products = result.Products != null ? [.. result.Products.Items] : [],
            Categories = [.. result.Categories],
        };

        return View(model);
    }
}
