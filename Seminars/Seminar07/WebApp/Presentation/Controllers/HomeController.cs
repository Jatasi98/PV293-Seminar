using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;
using System.Diagnostics;

namespace Presentation.Controllers;

public class HomeController : Controller
{
    private readonly ISearchService _searchService;

    public HomeController(ISearchService searchService)
    {
        _searchService = searchService;
    }

    public async Task<IActionResult> Index()
    {
        var categories = await _searchService.GetFeaturedCategories();

        var products = await _searchService.GetFeaturedProducts();

        var model = new HomeIndexViewModel
        {
            PopularCategories = [.. categories ?? []],
            FeaturedProducts = [.. products ?? []],
        };

        return View(model);
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
