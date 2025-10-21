using BL.Services;
using Microsoft.AspNetCore.Mvc;
using PV293WebApplication.Models;

namespace PV293WebApplication.Controllers.Admin;

[Area("Admin")]
public class CategoriesController : AdminController
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<IActionResult> Index()
    {
        var categories = await _categoryService.GetCategories();

        return View(categories);
    }

    public async Task<IActionResult> Details(int id)
    {
        var category = await _categoryService.FindCategoryById(id);
        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCategoryViewModel model)
    {
        if (ModelState.IsValid)
        {
            var category = new BL.DTOs.CategoryDTO()
            {
                Name = model.Name,
                Description = model.Description,
            };

            await _categoryService.CreateCategory(category);

            return RedirectToAction(nameof(Index));
        }

        return View(model);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var category = await _categoryService.FindCategoryById(id);
        if (category == null) 
        { 
            return NotFound();
        }

        return View(category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateCategoryViewModel model)
    {
        if (model.Id < 1) 
        {
            return BadRequest();
        }

        if (ModelState.IsValid)
        {
            var category = new BL.DTOs.CategoryDTO()
            {
                Name = model.Name,
                Description = model.Description,
            };

            await _categoryService.UpdateCategory(category);

            return RedirectToAction(nameof(Index));
        }

        return View(model);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        if (id < 1)
        {
            return NotFound();
        }

        await _categoryService.DeleteCategory(id);

        return RedirectToAction(nameof(Index));
    }
}
