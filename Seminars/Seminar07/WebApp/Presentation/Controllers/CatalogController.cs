using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Presentation.Models;

namespace Presentation.Controllers;

public class CatalogController : Controller
{
    private readonly ICatalogService _catalogService;

    public CatalogController(ICatalogService catalogService)
    {
        _catalogService = catalogService;
    }

    public async Task<IActionResult> Index([FromQuery] CatalogFilterViewModel filter)
    {
        if (filter.Page <= 0)
        {
            filter.Page = 1;
        }

        if (filter.PageSize <= 0 || filter.PageSize > 100)
        {
            filter.PageSize = 12;
        }

        var result = await _catalogService.GetCatalogResults(
                filter.Search,
                filter.CategoryId,
                filter.Sort,
                filter.Page,
                filter.PageSize);

        var viewModel = new CatalogPageViewModel
        {
            Filter = filter,
            Results = result.Products,
            Categories = [.. result.Categories
                .Select(category => new SelectListItem
                {
                    Value = category.Id.ToString(),
                    Text = category.Name
                })]
        };

        return View(viewModel);
    }
}
