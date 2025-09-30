using DAL;
using DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PV293WebApplication.Models;

namespace PV293WebApplication.Controllers;

public class CatalogController : Controller
{
    private readonly WebAppDbContext _db;
    public CatalogController(WebAppDbContext db) => _db = db;

    public async Task<IActionResult> Index([FromQuery] CatalogFilterViewModel filter)
    {
        if (filter.Page <= 0) filter.Page = 1;
        if (filter.PageSize <= 0 || filter.PageSize > 100) filter.PageSize = 12;

        var query = _db.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var s = filter.Search.Trim();
            query = query.Where(p => p.Name.Contains(s) || p.Description != null && p.Description.Contains(s));
        }

        if (filter.CategoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == filter.CategoryId);
        }

        query = filter.Sort switch
        {
            "name_desc" => query.OrderByDescending(p => p.Name),
            "price_asc" => query.OrderBy(p => p.Price).ThenBy(p => p.Name),
            "price_desc" => query.OrderByDescending(p => p.Price).ThenBy(p => p.Name),
            _ => query.OrderBy(p => p.Name),
        };

        var total = await query.CountAsync();

        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        var vm = new CatalogPageViewModel
        {
            Filter = filter,
            Results = new PagedResult<Product>
            {
                Items = items,
                TotalItems = total,
                Page = filter.Page,
                PageSize = filter.PageSize
            },
            Categories = await _db.Categories
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                .ToListAsync()
        };

        return View(vm);
    }
}
